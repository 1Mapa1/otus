using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.GetOrderById
{
    public sealed record OrderDetailsDto(
        Guid Id,
        string Status,
        string? FailureReason,
        decimal Price,
        DateTime CreatedAt);
}
