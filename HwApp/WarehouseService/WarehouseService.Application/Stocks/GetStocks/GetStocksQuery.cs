using WarehouseService.Application.Common;
using MediatR;

namespace WarehouseService.Application.Stocks.GetStocks
{
    public sealed record GetStocksQuery() : IRequest<Result<GetStocksResult>>;
}
