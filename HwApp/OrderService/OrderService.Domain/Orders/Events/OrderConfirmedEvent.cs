using OrderService.Domain.Events;

namespace OrderService.Domain.Orders.Events
{
    public sealed record OrderConfirmedEvent(
            Guid OrderId,
            Guid UserId,
            Guid DeliverySlotId,
            decimal TotalAmount,
            IReadOnlyCollection<OrderItem> Items
        ) : IDomainEvent
    {
        public string Key => OrderId.ToString();
    }
}
