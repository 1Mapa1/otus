namespace CatalogService.Application.Products.GetProductSnapshot
{
    public sealed record PriceChangedItem(
        Guid ProductId,
        decimal ExpectedUnitPrice,
        decimal ActualUnitPrice);
}
