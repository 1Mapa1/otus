using OrderService.Domain.Events;
using OrderService.Domain.Orders.Events;

namespace OrderService.Domain.Orders
{
    public sealed class Order : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _events = [];

        private readonly List<OrderItem> _items = [];

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid DeliverySlotId { get; private set; }

        public decimal TotalAmount { get; private set; }

        public OrderStatus Status { get; private set; }

        public OrderSagaStep SagaStep { get; private set; }

        public Guid? PaymentId { get; private set; }

        public Guid? StockReservationId { get; private set; }

        public Guid? DeliveryReservationId { get; private set; }

        public OrderFailureReason? FailureReason { get; private set; }

        public string? FailureDetails { get; private set; }

        public int RetryCount { get; private set; }

        public DateTime? NextRetryAt { get; private set; }

        public string? LockedBy { get; private set; }

        public DateTime? LockedUntil { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public DateTime? RejectedAt { get; private set; }

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        public IReadOnlyCollection<OrderItem> Items => _items;

        private Order() { }

        public static Order Create(Guid userId, Guid deliverySlotId, decimal totalAmount)
        {
            var now = DateTime.UtcNow;

            return new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DeliverySlotId = deliverySlotId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Processing,
                SagaStep = OrderSagaStep.Created,
                RetryCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public void AddItem(
            Guid productId,
            string name,
            decimal unitPrice,
            int quantity,
            decimal totalPrice)
        {
            _items.Add(OrderItem.Create(
                Id,
                productId,
                name,
                unitPrice,
                quantity,
                totalPrice));
        }

        public void MarkAsConfirmed()
        {
            var now = DateTime.UtcNow;

            Status = OrderStatus.Confirmed;
            SagaStep = OrderSagaStep.Completed;
            CompletedAt = now;
            UpdatedAt = now;
            AddEvent(new OrderConfirmedEvent(
                Id,
                UserId,
                DeliverySlotId,
                TotalAmount,
                _items));
        }

        public void MarkAsRejected(OrderFailureReason failureReason)
        {
            var now = DateTime.UtcNow;

            Status = OrderStatus.Rejected;
            SagaStep = OrderSagaStep.Compensating;
            UpdatedAt = now;
            RejectedAt = now;
            FailureReason = failureReason;
            AddEvent(new OrderRejectedEvent(
                Id,
                UserId,
                DeliverySlotId,
                TotalAmount,
                FailureReason.ToString(),
                FailureDetails,
                _items));
        }

        public void AddEvent(IDomainEvent domainEvent)
            => _events.Add(domainEvent);

        public void ClearEvents()
            => _events.Clear();
    }
}
