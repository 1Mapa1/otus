namespace OrderService.Infrastructure.Clients.Catalog.Responses
{
    internal sealed record GetProductSnapshotResponse(
        IReadOnlyCollection<ProductSnapshotResponseItem> Items,
        decimal TotalAmount);
}
