namespace OrderService.Application.Orders.GetOrderById
{
    public sealed record OrderItemDetailsDto(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
