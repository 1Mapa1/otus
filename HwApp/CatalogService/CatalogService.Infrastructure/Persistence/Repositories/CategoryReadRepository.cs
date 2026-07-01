using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class CategoryReadRepository : ICategoryReadRepository
    {
        private readonly CatalogReadDbContext _context;

        public CategoryReadRepository(CatalogReadDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CategoryListItemDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .Select(category => new CategoryListItemDto(category.Id, category.Name))
                .ToListAsync(cancellationToken);
        }
    }
}
