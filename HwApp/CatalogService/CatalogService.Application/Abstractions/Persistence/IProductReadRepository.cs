using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface IProductReadRepository
    {
        Task<ProductListResultDto> GetProductsAsync(
            ProductListQuery query,
            CancellationToken cancellationToken = default);

        Task<ProductDetailsDto?> GetProductByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}
