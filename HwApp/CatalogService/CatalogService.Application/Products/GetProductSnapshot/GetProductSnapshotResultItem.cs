namespace CatalogService.Application.Products.GetProductSnapshot
{
    public sealed record GetProductSnapshotResultItem(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
