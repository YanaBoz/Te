using Microsoft.EntityFrameworkCore;
using Web_Library.Data;
using Web_Library.Models;

namespace Web_Library.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<Book>> GetBorrowedBooksAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.BorrowedBooks)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            return user?.BorrowedBooks.ToList() ?? new List<Book>();
        }
    }
}

