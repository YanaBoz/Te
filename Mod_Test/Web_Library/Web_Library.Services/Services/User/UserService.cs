using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Repositories;

namespace Web_Library.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role
            };
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            return await _userRepository.AuthenticateAsync(username, password);
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                Username = registerDto.Username,
                FullName = registerDto.FullName
            };
            return await _userRepository.RegisterAsync(user, registerDto.Password);
        }

        public async Task<List<Book>> GetBorrowedBooksAsync(string userId)
        {
            return await _userRepository.GetBorrowedBooksAsync(userId);
        }

        public async Task<UserDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role
            };
        }
    }
}
