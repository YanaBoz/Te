using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web_Library.API.Models;

namespace Web_Library.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        // Для демонстрационных целей refresh-токены храним в памяти.
        // В реальном приложении рекомендуется использовать хранилище (БД).
        private static readonly ConcurrentDictionary<string, string> _refreshTokens = new();

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                // "sub" – идентификатор субъекта (username)
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                // Идентификатор токена (GUID)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Стандартный claim для идентификатора пользователя
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                // Роль пользователя
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(string userId)
        {
            var refreshToken = Guid.NewGuid().ToString();
            _refreshTokens[userId] = refreshToken;
            return refreshToken;
        }

        public bool ValidateRefreshToken(string userId, string refreshToken)
        {
            return _refreshTokens.TryGetValue(userId, out var storedToken) && storedToken == refreshToken;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // При валидации устаревшего токена отключаем проверку времени жизни
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // отключено для извлечения данных из просроченного токена
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]))
            }, out _);

            return principal;
        }
    }
}
