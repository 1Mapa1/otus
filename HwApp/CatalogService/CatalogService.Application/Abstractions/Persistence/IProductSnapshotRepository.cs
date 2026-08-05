namespace CatalogService.Application.Abstractions.Persistence
{
    public interface IProductSnapshotRepository
    {
        Task<IReadOnlyList<ProductSnapshotRow>> GetActiveProductsAsync(
            IReadOnlyList<Guid> productIds,
            CancellationToken cancellationToken = default);
    }
}
