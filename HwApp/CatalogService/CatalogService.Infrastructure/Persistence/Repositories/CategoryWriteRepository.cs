using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Domain.Categories;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class CategoryWriteRepository : ICategoryWriteRepository
    {
        private readonly CatalogWriteDbContext _context;

        public CategoryWriteRepository(CatalogWriteDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            return _context.Categories.AddAsync(category, cancellationToken).AsTask();
        }

        public Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return _context.Categories.FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken);
        }

        public Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim();

            return _context.Categories.AsNoTracking()
                .AnyAsync(
                    category => category.Name == normalized && (excludeCategoryId == null || category.Id != excludeCategoryId),
                    cancellationToken);
        }
    }
}
