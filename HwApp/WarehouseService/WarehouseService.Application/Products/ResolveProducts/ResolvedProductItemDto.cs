namespace WarehouseService.Application.Products.ResolveProducts
{
    public sealed record ResolvedProductItemDto(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
