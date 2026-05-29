using OrderService.Domain.Events;
using OrderService.Domain.Orders.Events;

namespace OrderService.Domain.Orders
{
    public sealed class Order : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _events = [];

        private readonly List<OrderItem> _items = [];

        private static DateTime Now => DateTime.UtcNow;

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
            var now = Now;

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

        public void MarkAsPaymentAuthorized(Guid paymetnId)
        {
            Status = OrderStatus.Processing;
            SagaStep = OrderSagaStep.PaymentAuthorized;
            UpdatedAt = Now;
            PaymentId = paymetnId;
        }

        public void MarkAsStockReserved(Guid stockReservationId)
        {
            Status = OrderStatus.Processing;
            SagaStep = OrderSagaStep.StockReserved;
            UpdatedAt = Now;
            StockReservationId = stockReservationId;
        }

        public void MarkAsDeliveryReserved(Guid deliveryReservationId)
        {
            Status = OrderStatus.Processing;
            SagaStep = OrderSagaStep.DeliveryReserved;
            UpdatedAt = Now;
            DeliveryReservationId = deliveryReservationId;
        }

        public void MarkAsConfirmed()
        {
            var now = Now;

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

        public void MarkAsCompensating(OrderFailureReason failureReason, string failureDetails)
        {
            Status = OrderStatus.Processing;
            SagaStep = OrderSagaStep.Compensating;
            UpdatedAt = Now;
            FailureReason = failureReason;
            FailureDetails = failureDetails;
        }

        public void MarkAsRejected(OrderFailureReason failureReason, string failureDetails)
        {
            var now = Now;

            Status = OrderStatus.Rejected;
            UpdatedAt = now;
            RejectedAt = now;
            FailureReason = failureReason;
            FailureDetails = failureDetails;
            AddEvent(new OrderRejectedEvent(
                Id,
                UserId,
                DeliverySlotId,
                TotalAmount,
                FailureReason.ToString(),
                FailureDetails,
                _items));
        }

        public void MarkAsCompensated()
        {
            var now = Now;

            Status = OrderStatus.Rejected;
            SagaStep = OrderSagaStep.Compensated;
            UpdatedAt = now;
            RejectedAt = now;
            AddEvent(new OrderRejectedEvent(
                Id,
                UserId,
                DeliverySlotId,
                TotalAmount,
                FailureReason.ToString(),
                FailureDetails,
                _items));
        }

        public void MarkAsCompensationFailed(string details)
        {
            Status = OrderStatus.CompensationFailed;
            SagaStep = OrderSagaStep.CompensationFailed;
            FailureDetails = details;
            UpdatedAt = Now;
        }

        public void ReleaseLock()
        {
            LockedBy = null;
            LockedUntil = null;

            RetryCount = 0;
            NextRetryAt = null;
        }

        public void ScheduleRetry(string failureDetails, TimeSpan nextRetryAt)
        {
            RetryCount = RetryCount++;
            NextRetryAt = Now.Add(nextRetryAt);
        }


        public void AddEvent(IDomainEvent domainEvent)
            => _events.Add(domainEvent);

        public void ClearEvents()
            => _events.Clear();
    }
}
