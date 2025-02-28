using Web_Library.DTOs;
using Web_Library.Models;

namespace Web_Library.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<List<Book>> GetBorrowedBooksAsync(string userId);
        Task<UserDto?> GetUserProfileAsync(string userId);
    }
}
