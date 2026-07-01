namespace OrderService.Infrastructure.Clients.Catalog.Responses
{
    internal sealed record CatalogErrorResponse(
        string? ErrorCode,
        string? Code,
        string? Message,
        IReadOnlyList<CatalogPriceChangedItemResponse>? Items);
}
