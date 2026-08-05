using WarehouseService.Application.Common;
using MediatR;

namespace WarehouseService.Application.Stocks.GetStockMovements
{
    public sealed record GetStockMovementsQuery(Guid ProductId) : IRequest<Result<GetStockMovementsResult>>;
}
