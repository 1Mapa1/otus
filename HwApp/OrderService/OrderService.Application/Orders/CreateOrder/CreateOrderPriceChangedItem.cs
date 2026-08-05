namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderPriceChangedItem(
        Guid ProductId,
        decimal ExpectedUnitPrice,
        decimal ActualUnitPrice);
}
