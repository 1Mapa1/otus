using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.GetOrderById
{
    public sealed record OrderDetailsDto(
        Guid Id,
        OrderStatus Status,
        OrderFailureReason? FailureReason,
        decimal Price,
        DateTime CreatedAt);
}
