namespace OrderService.Application.Abstractions.Clients.Catalog
{
    public interface ICatalogClient
    {
        Task<GetProductSnapshotResult> GetSnapshotAsync(
            IReadOnlyCollection<GetProductSnapshotItem> items,
            CancellationToken cancellationToken = default);
    }
}
