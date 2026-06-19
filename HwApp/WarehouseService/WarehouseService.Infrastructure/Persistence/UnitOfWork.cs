using WarehouseService.Application.Abstractions;

namespace WarehouseService.Infrastructure.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _databaseContext;

        public UnitOfWork(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}
