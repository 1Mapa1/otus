namespace OrderService.Application.Abstractions.Clients.Catalog
{
    public sealed record CatalogSnapshotItem(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
