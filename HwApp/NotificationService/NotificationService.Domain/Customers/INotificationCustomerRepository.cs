namespace NotificationService.Domain.Customers
{
    public interface INotificationCustomerRepository
    {
        public Task<NotificationCustomer?> GetByIdAsync(Guid id, CancellationToken ct = default);

        public Task AddAsync(NotificationCustomer customer, CancellationToken ct = default);
    }
}
