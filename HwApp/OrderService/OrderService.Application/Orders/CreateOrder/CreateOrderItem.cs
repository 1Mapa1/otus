namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderItem(
        Guid ProductId,
        int Quantity);
}
