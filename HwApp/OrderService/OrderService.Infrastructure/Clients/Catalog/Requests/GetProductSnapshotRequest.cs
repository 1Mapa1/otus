namespace OrderService.Infrastructure.Clients.Catalog.Requests
{
    internal sealed record GetProductSnapshotRequest(
        IReadOnlyCollection<ProductSnapshotItemDto> Items);
}
