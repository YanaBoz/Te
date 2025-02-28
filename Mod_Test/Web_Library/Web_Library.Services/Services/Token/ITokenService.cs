using System.Security.Claims;
using Web_Library.Models;

namespace Web_Library.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(string userId);
        bool ValidateRefreshToken(string userId, string refreshToken);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
