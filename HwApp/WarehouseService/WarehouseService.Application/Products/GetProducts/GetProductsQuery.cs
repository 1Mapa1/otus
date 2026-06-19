using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Products.GetProducts
{
    public sealed record GetProductsQuery 
        : IRequest<Result<GetProductsResult>>;
}
