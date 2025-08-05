
using System.Security.Claims;

namespace FigureGear.Service.Interface
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);

        string GenerateRefreshToken();
    }
}
