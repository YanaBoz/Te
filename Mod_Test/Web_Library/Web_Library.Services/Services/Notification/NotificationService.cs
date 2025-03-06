using Web_Library.Models;
using Web_Library.Repositories.Repositories.NotiRepo;

namespace Web_Library.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationService(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> UserHasOverdueBooksAsync(string userId, CancellationToken cancellationToken)
        {
            var overdueBooks = await _notificationRepository.GetOverdueBooksAsync(userId, cancellationToken);
            return overdueBooks.Any();
        }
    }
}
