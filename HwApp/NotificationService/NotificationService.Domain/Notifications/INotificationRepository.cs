namespace NotificationService.Domain.Notifications
{
    public interface INotificationRepository
    {
        public Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

        public Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
