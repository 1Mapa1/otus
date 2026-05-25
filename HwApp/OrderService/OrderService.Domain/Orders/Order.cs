using OrderService.Domain.Events;
using OrderService.Domain.Orders.Events;

namespace OrderService.Domain.Orders
{
    public sealed class Order : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _events = [];

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public decimal Price { get; private set; }

        public OrderStatus Status { get; private set; }

        public OrderFailureReason? FailureReason { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        private Order() { }

        public static Order Create(Guid userId, decimal price)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Price = price,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsPaid()
        {
            Status = OrderStatus.Paid;
            UpdatedAt = DateTime.UtcNow;
            AddEvent(new OrderPaidEvent(Id, UserId, Price));
        }

        public void MarkAsRejected(OrderFailureReason failureReason)
        {
            Status = OrderStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
            FailureReason = failureReason;
            AddEvent(new OrderRejectedEvent(Id, UserId, Price, FailureReason?.ToString() ?? string.Empty));
        }

        public void AddEvent(IDomainEvent domainEvent)
            => _events.Add(domainEvent);

        public void ClearEvents()
            => _events.Clear();
    }
}
