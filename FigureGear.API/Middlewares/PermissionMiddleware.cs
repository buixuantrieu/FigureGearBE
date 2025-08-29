using FigureGear.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using FigureGear.Service.Helpers;
using FigureGear.Service.Interface.UserInterface;

namespace FigureGear.API.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
                                      ICurrentUserService currentUserService,
                                      IUserContextService userContextService)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            if (!context.User.Identity?.IsAuthenticated ?? false)
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

            var userInfo = await userContextService.GetUserInfoAsync(userId.Value);
            if (userInfo is null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }

            var (roleName, securityStamp) = userInfo.Value;

            if (roleName == RoleNames.Admin)
            {
                await _next(context);
                return;
            }

            var tokenSecurityStamp = context.User.Claims
                .FirstOrDefault(c => c.Type == "security_stamp")?.Value;

            if (tokenSecurityStamp != securityStamp)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var requiredPermissions = endpoint?.Metadata
                .GetOrderedMetadata<RequirePermissionAttribute>()
                .SelectMany(attr => attr.Permissions)
                .ToList();

            if (requiredPermissions != null && requiredPermissions.Any())
            {
                var userClaims = context.User.Claims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToHashSet();

                if (!requiredPermissions.Any(p => userClaims.Contains(p)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                    return;
                }
            }

            await _next(context);
        }
    }
}
