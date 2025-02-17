using System.Security.Claims;
using Web_Library.API.Models;

namespace Web_Library.API.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(string userId);
        bool ValidateRefreshToken(string userId, string refreshToken);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
