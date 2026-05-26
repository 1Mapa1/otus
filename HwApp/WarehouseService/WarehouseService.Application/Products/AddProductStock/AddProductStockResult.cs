namespace WarehouseService.Application.Products.AddProductStock
{
    public sealed record AddProductStockResult(
        Guid ProductId,
        int AvailableQuantity,
        int ReservedQuantity,
        int FreeQuantity);
}