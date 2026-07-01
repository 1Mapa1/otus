using CatalogService.Domain.Categories;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface ICategoryWriteRepository
    {
        Task AddAsync(Category category, CancellationToken cancellationToken = default);

        Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeCategoryId = null,
            CancellationToken cancellationToken = default);
    }
}
