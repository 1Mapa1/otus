using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    internal sealed class NotificationRepository : INotificationRepository
    {
        private readonly DatabaseContext _dbContext;

        public NotificationRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
        {
            await _dbContext.Notifications.AddAsync(notification, ct);
        }

        public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _dbContext.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Notifications
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}
