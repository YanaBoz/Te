using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(string userId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string userId);
        Task<List<Book>> GetBorrowedBooksAsync(string userId);
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
    }
}
