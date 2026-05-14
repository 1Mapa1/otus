using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.GetMyOrders
{
    public sealed record OrderDto(
        Guid Id,
        string Status,
        DateTime CreatedAt);
}
