using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Customers;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    internal sealed class NotificationCustomerRepository : INotificationCustomerRepository
    {
        private readonly DatabaseContext _dbContext;

        public NotificationCustomerRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(NotificationCustomer customer, CancellationToken ct)
        {
            await _dbContext.NotificationCustomers.AddAsync(customer, ct);
        }

        public async Task<NotificationCustomer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.NotificationCustomers
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}
