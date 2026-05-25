using OrderService.Domain.Events;

namespace OrderService.Domain.Orders.Events
{
    public sealed record OrderRejectedEvent(
            Guid OrderId,
            Guid UserId,
            decimal Price,
            string FailureReason
        ) : IDomainEvent
    {
        public string Key => OrderId.ToString();
    }
}
