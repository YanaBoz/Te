using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(string userId, CancellationToken cancellationToken);
        Task<List<Book>> GetBorrowedBooksAsync(string userId, CancellationToken cancellationToken);
    }
}
