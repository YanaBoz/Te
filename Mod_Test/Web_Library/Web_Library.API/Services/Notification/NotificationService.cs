using Microsoft.EntityFrameworkCore;
using Web_Library.API.Data;

namespace Web_Library.API.Services.Notification
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
                // Логика отправки уведомления
                book.IsNotified = true; // Пометить как уведомленный
            }

            await _context.SaveChangesAsync();
        }
    }
}
