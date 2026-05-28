using OrderService.Domain.Events;

namespace OrderService.Domain.Orders.Events
{
    public sealed record OrderRejectedEvent(
            Guid OrderId,
            Guid UserId,
            Guid DeliverySlotId,
            decimal TotalAmount,
            string? FailureReason,
            string? FailureDetails,
            IReadOnlyCollection<OrderItem> Items
        ) : IDomainEvent
    {
        public string Key => OrderId.ToString();
    }
}
