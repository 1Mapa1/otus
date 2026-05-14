using OrderService.Domain.Events;
using System.Security.Cryptography;

namespace OrderService.Domain.Orders.Events
{
    public sealed record OrderPaidEvent(
            Guid OrderId,
            Guid UserId,
            decimal Price
        ) : IDomainEvent
    {
        public string Key => OrderId.ToString();
    }
}
