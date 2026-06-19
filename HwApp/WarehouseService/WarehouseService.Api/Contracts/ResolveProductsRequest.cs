using WarehouseService.Application.Products.ResolveProducts;

namespace WarehouseService.Api.Contracts
{
    public sealed record ResolveProductsRequest(
        List<ResolveProductItem> Items);
}
