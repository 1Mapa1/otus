namespace OrderService.Application.Abstractions.Clients.Catalog
{
    public sealed record CatalogPriceChangedItem(
        Guid ProductId,
        decimal ExpectedUnitPrice,
        decimal ActualUnitPrice);
}
