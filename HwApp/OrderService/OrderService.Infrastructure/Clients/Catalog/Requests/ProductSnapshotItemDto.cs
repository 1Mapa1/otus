namespace OrderService.Infrastructure.Clients.Catalog.Requests
{
    internal sealed record ProductSnapshotItemDto(
        Guid ProductId,
        int Quantity,
        decimal ExpectedUnitPrice);
}
