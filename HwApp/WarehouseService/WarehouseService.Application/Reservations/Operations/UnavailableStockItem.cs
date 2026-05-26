namespace WarehouseService.Application.Reservations.Operations
{
    public sealed record UnavailableStockItem(
        Guid ProductId,
        int RequestedQuantity,
        int FreeQuantity);
}
