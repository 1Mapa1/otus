using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Products.AddProductStock
{
    public sealed record AddProductStockCommand(
        Guid ProductId,
        int Quantity) : IRequest<Result<AddProductStockResult>>;
}
