using NotificationService.Application.Abstractions;

namespace NotificationService.Infrastructure.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return _dbContext.SaveChangesAsync(ct);
        }
    }
}
