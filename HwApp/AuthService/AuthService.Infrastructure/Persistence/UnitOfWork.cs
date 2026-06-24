using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Rersistence;

namespace AuthService.Infrastructure.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _dbContext;

        public UnitOfWork(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
