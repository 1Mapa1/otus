namespace OrderService.Application.Abstractions.Clients.Catalog
{
    public sealed record CatalogClientError(
        CatalogClientErrorCode Code,
        string? Message,
        IReadOnlyList<CatalogPriceChangedItem>? PriceChangedItems = null);
}
