using Web_Library.Data;
using Web_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Web_Library.Repositories.Repositories.NotiRepo
{
    public class NotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetOverdueBooksAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.BorrowedBooks)
                .ThenInclude(b => b.GenreNavigation) 
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                return new List<Book>(); 
            }

            return user.BorrowedBooks
                .Where(b => b.ReturnBy < DateTime.Now && !b.IsNotified && b.Quantity > 0)
                .ToList();
        }
    }
}
