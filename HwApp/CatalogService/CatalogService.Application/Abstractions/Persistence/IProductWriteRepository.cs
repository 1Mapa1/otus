using CatalogService.Domain.Products;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface IProductWriteRepository
    {
        Task AddAsync(
            Product product,
            string brandName,
            string attributeValuesJson,
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdWithAttributesAsync(Guid productId, CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Product product,
            string brandName,
            string attributeValuesJson,
            CancellationToken cancellationToken = default);

        Task ArchiveAsync(Product product, CancellationToken cancellationToken = default);

        Task RestoreAsync(Product product, CancellationToken cancellationToken = default);

        Task UpdateBrandNameInReadModelsAsync(
            Guid brandId,
            string brandName,
            CancellationToken cancellationToken = default);
    }
}
