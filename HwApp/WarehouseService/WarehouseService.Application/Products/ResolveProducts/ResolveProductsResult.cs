namespace WarehouseService.Application.Products.ResolveProducts
{
    public sealed record ResolveProductsResult(
        IReadOnlyList<ResolvedProductItemDto> Items,
        decimal TotalAmount);
}
