using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Products.CreateProduct
{
    public sealed record CreateProductCommand(
        string Name,
        decimal UnitPrice) : IRequest<Result<CreateProductResult>>;
}
