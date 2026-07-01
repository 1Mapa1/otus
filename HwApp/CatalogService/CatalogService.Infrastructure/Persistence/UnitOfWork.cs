using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Infrastructure.Persistence;

namespace CatalogService.Infrastructure.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogWriteDbContext _context;

        public UnitOfWork(CatalogWriteDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
