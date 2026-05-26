namespace WarehouseService.Application.Products.ResolveProducts
{
    public sealed record ResolveProductItem(
        Guid ProductId,
        int Quantity);
}
