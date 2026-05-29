namespace WarehouseService.Application.Reservations.Operations
{
    public sealed record StockNotAvailableError(
        string ErrorCode,
        IReadOnlyList<UnavailableStockItem> UnavailableItems);
}
