namespace CatalogService.Application.Products.GetProductSnapshot
{
    public sealed record GetProductSnapshotResult(
        IReadOnlyList<GetProductSnapshotResultItem> Items,
        decimal TotalAmount);
}
