using OrderService.Infrastructure.Clients.Warehouse.Dto;

namespace OrderService.Infrastructure.Clients.Warehouse.Requests
{
    internal sealed record ResolveProductsRequest(
        IEnumerable<ProductQuantityDto> Items);
}
