namespace WarehouseService.Application.Stocks.GetStocks
{
    public sealed record GetStocksResult(IReadOnlyList<StockItemDto> Items);
}
