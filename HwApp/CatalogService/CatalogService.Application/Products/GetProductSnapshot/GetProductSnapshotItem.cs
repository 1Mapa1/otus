namespace CatalogService.Application.Products.GetProductSnapshot
{
    public sealed record GetProductSnapshotItem(Guid ProductId, int Quantity, decimal? ExpectedUnitPrice);
}
