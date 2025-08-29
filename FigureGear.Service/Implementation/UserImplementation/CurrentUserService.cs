using System.Security.Claims;
using FigureGear.Service.Interface.UserInterface;
using FigureGear.Service.Shared;
using Microsoft.AspNetCore.Http;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return null;

            return Guid.TryParse(userIdString, out var guid) ? guid : null;
        }
    }
}
