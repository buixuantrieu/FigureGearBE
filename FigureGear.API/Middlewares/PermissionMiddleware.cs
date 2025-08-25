using FigureGear.API.Attributes;
using FigureGear.Service.Interface;
using FigureGear.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;

namespace FigureGear.API.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }
            var currentUserService = context.RequestServices.GetRequiredService<ICurrentUserService>();
            var dbContext = context.RequestServices.GetRequiredService<FigureGearDbContext>();
            var cache = context.RequestServices.GetService<IDistributedCache>();

            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var userId = currentUserService.UserId;
            if (!userId.HasValue)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }

            var tokenSecurityStamp = context.User.Claims
                .FirstOrDefault(c => c.Type == "security_stamp")?.Value;

            if (!await IsSecurityStampValidAsync(dbContext, cache, userId.Value, tokenSecurityStamp))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var requiredPermissions = endpoint?.Metadata
                .GetOrderedMetadata<RequirePermissionAttribute>()
                .Select(attr => attr.Permission)
                .ToList();

            if (requiredPermissions != null && requiredPermissions.Any())
            {
                var userClaims = context.User.Claims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToHashSet();

                foreach (var permission in requiredPermissions)
                {
                    if (!userClaims.Contains(permission))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Forbidden");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private async Task<bool> IsSecurityStampValidAsync(
            FigureGearDbContext dbContext,
            IDistributedCache cache,
            Guid userId,
            string tokenSecurityStamp)
        {
            if (string.IsNullOrEmpty(tokenSecurityStamp))
                return false;

            if (cache != null)
            {
                var cachedStamp = await cache.GetStringAsync($"SecurityStamp:{userId}");
                if (cachedStamp != null)
                    return cachedStamp == tokenSecurityStamp;
            }

            var userSecurityStamp = await dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.SecurityStamp)
                .FirstOrDefaultAsync();

            if (cache != null && userSecurityStamp != null)
            {
                await cache.SetStringAsync($"SecurityStamp:{userId}", userSecurityStamp,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
            }

            return tokenSecurityStamp == userSecurityStamp;
        }
    }
}
