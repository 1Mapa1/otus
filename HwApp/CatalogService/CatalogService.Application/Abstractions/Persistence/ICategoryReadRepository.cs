using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface ICategoryReadRepository
    {
        Task<IReadOnlyList<CategoryListItemDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default);
    }
}
