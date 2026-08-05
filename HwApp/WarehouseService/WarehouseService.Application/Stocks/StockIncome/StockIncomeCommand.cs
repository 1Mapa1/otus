using WarehouseService.Application.Common;
using MediatR;

namespace WarehouseService.Application.Stocks.StockIncome
{
    public sealed record StockIncomeCommand(Guid ProductId, int Quantity)
        : IRequest<Result<StockIncomeResult>>;
}
