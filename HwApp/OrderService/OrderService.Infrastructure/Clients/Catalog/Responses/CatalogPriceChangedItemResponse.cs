namespace OrderService.Infrastructure.Clients.Catalog.Responses
{
    internal sealed record CatalogPriceChangedItemResponse(
        Guid ProductId,
        decimal ExpectedUnitPrice,
        decimal ActualUnitPrice);
}
