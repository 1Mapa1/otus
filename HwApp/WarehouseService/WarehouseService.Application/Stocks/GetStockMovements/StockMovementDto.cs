namespace WarehouseService.Application.Stocks.GetStockMovements
{
    public sealed record StockMovementDto(
        Guid Id,
        Guid ProductId,
        string Type,
        int Quantity,
        Guid? OrderId,
        DateTime CreatedAt);
}
