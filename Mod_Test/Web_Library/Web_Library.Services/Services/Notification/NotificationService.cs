using Microsoft.EntityFrameworkCore;
using Web_Library.Data;
using Web_Library.Repositories;

namespace Web_Library.Services.Notification
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendOverdueNotifications()
        {
            var overdueBooks = await _context.Books
                .Where(b => b.ReturnBy < DateTime.Now && !b.IsNotified)
                .ToListAsync();

            foreach (var book in overdueBooks)
            {
                book.IsNotified = true; 
            }

            await _context.SaveChangesAsync();
        }
    }
}
