namespace WarehouseService.Application.Stocks.StockIncome
{
    public sealed record StockIncomeResult(
        Guid ProductId,
        int AvailableQuantity,
        int ReservedQuantity,
        bool IsActive,
        DateTime UpdatedAt);
}
