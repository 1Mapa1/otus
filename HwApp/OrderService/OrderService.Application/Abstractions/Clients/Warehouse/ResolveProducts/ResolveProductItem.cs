namespace OrderService.Application.Abstractions.Clients.Warehouse.ResolveProducts
{
    public sealed record ResolveProductItem(
        Guid ProductId,
        int Quantity);
}
