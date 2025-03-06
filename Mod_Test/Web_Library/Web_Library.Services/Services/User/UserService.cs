using System.Security.Claims;
using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Repositories;
using Web_Library.Services.Services.Password;
using Mapster;
using Web_Library.Services.Notification;
using System.Threading;

namespace Web_Library.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        public UserService(IUserRepository userRepository, ITokenService tokenService, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }
        public async Task<RefreshTokenDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username, cancellationToken);
            if (user == null || !_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
                return null;

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
            return new RefreshTokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (user == null) return null;

            return user.Adapt<UserDto>();
        }

        public async Task<RefreshTokenDto?> RefreshTokenAsync(RefreshTokenDto tokenDto, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !_tokenService.ValidateRefreshToken(userId, tokenDto.RefreshToken))
                return null;

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) return null;

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(userId);
            return new RefreshTokenDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetByUsernameAsync(registerDto.Username, cancellationToken) != null)
                return false;

            var user = registerDto.Adapt<User>();
            user.PasswordHash = _passwordService.HashPassword(registerDto.Password);

            await _userRepository.AddAsync(user, cancellationToken);
            return true;
        }

        public async Task<List<Book>> GetBorrowedBooksAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userRepository.GetBorrowedBooksAsync(userId, cancellationToken);
        }

        public async Task<UserDto?> GetUserProfileAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) return null;

            return user.Adapt<UserDto>();
        }
    }
}
