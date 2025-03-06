namespace Web_Library.Services.Notification
{
    public interface INotificationService
    {
        public Task<bool> UserHasOverdueBooksAsync(string userId, CancellationToken cancellationToken);
    }
}
