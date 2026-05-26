using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Products.ResolveProducts
{
    public sealed record ResolveProductsCommand(
        List<ResolveProductItem> Items) : IRequest<Result<ResolveProductsResult>>;
}