namespace WarehouseService.Application.Stocks.GetStockById
{
    public sealed record GetStockByIdResult(
        Guid ProductId,
        int AvailableQuantity,
        int ReservedQuantity,
        bool IsActive,
        DateTime UpdatedAt);
}
