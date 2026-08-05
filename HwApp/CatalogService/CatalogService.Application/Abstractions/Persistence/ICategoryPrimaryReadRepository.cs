using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface ICategoryPrimaryReadRepository
    {
        Task<IReadOnlyList<CategoryListItemDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default);
    }
}
