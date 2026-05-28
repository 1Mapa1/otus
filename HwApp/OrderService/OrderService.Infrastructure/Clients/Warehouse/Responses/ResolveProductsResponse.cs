using OrderService.Infrastructure.Clients.Warehouse.Dto;

namespace OrderService.Infrastructure.Clients.Warehouse.Responses
{
    internal sealed record ResolveProductsResponse(
        IReadOnlyList<ProductDetailsDto> Items,
        decimal TotalAmount);
}