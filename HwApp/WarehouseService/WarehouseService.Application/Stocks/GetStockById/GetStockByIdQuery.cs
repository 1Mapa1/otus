using WarehouseService.Application.Common;
using MediatR;

namespace WarehouseService.Application.Stocks.GetStockById
{
    public sealed record GetStockByIdQuery(Guid ProductId) : IRequest<Result<GetStockByIdResult>>;
}
