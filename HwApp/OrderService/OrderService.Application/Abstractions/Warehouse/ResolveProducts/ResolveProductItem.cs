namespace OrderService.Application.Abstractions.Warehouse.ResolveProducts
{
    public sealed record ResolveProductItem(
        Guid ProductId,
        int Quantity);
}
