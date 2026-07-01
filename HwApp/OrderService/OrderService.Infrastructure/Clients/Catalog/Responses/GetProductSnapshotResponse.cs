namespace OrderService.Infrastructure.Clients.Catalog.Responses
{
    internal sealed record GetProductSnapshotResponse(
        IReadOnlyCollection<ProductSnapshotResponseItem> Items,
        decimal TotalAmount);

    internal sealed record ProductSnapshotResponseItem(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
