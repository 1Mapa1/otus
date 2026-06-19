using WarehouseService.Domain.Products;

namespace WarehouseService.Application.Products
{
    public interface IProductRepository
    {
        Task AddAsync(Product product, CancellationToken cancellationToken);

        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);

        Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken);

        Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IEnumerable<Guid> enumerable, CancellationToken cancellationToken);
    }
}
