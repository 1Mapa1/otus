namespace OrderService.Application.Abstractions.Warehouse.ResolveProducts
{
    public sealed record ResolvedProductItem(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
