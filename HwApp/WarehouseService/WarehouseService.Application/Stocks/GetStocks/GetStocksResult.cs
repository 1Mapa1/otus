namespace WarehouseService.Application.Stocks.GetStocks
{
    public sealed record StockItemDto(
        Guid ProductId,
        int AvailableQuantity,
        int ReservedQuantity,
        bool IsActive,
        DateTime UpdatedAt);

    public sealed record GetStocksResult(IReadOnlyList<StockItemDto> Items);
}
