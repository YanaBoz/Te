using System.Threading;
using Web_Library.DTOs;
using Web_Library.Models;

namespace Web_Library.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<bool> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);
        Task<List<Book>> GetBorrowedBooksAsync(string userId, CancellationToken cancellationToken);
        Task<UserDto?> GetUserProfileAsync(string userId, CancellationToken cancellationToken);
        Task<RefreshTokenDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);

        Task<RefreshTokenDto?> RefreshTokenAsync(RefreshTokenDto tokenDto, CancellationToken cancellationToken);
    }
}
