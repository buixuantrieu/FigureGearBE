using FigureGear.Data.Context;
using FigureGear.Service.Interface.UserInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FigureGear.Service.Implementation.UserImplementation
{
    public class UserContextService : DbContextService, IUserContextService
    {
        private readonly IDistributedCache? _cache;

        public UserContextService(FigureGearDbContext dbContext, IDistributedCache? cache):base(dbContext)
        {
            _cache = cache;
        }

        public async Task<(string RoleName, string SecurityStamp)?> GetUserInfoAsync(Guid userId)
        {
            string cacheKey = $"UserInfo:{userId}";
            string? cachedUser = _cache != null ? await _cache.GetStringAsync(cacheKey) : null;

            if (!string.IsNullOrEmpty(cachedUser))
            {
                var parts = cachedUser.Split(';');
                return (parts[0], parts[1]);
            }

            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Role)
                .Select(u => new { RoleName = u.Role.Name, u.SecurityStamp })
                .FirstOrDefaultAsync();

            if (user == null) return null;

            if (_cache != null)
            {
                await _cache.SetStringAsync(cacheKey, $"{user.RoleName};{user.SecurityStamp}",
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
            }

            return (user.RoleName, user.SecurityStamp);
        }
    }
}
