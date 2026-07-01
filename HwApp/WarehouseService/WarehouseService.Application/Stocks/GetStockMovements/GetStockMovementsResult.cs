namespace WarehouseService.Application.Stocks.GetStockMovements
{
    public sealed record GetStockMovementsResult(IReadOnlyList<StockMovementDto> Items);
}
