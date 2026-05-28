namespace OrderService.Application.Orders.GetOrderById
{
    public sealed record OrderDetailsDto(
        Guid Id,
        string Status,
        string SagaStep,
        decimal TotalAmount,
        IReadOnlyCollection<OrderItemDetailsDto> OrderDetails,
        Guid DeliverySlotId,
        string? FailureReason,
        DateTime CreatedAt);
}
