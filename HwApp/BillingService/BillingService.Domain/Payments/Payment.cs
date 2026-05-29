namespace BillingService.Domain.Payments
{
    public sealed class Payment
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid UserId { get; private set; }

        public decimal Amount { get; private set; }

        public PaymentStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public DateTime? AuthorizedAt { get; private set; }

        public DateTime? CapturedAt { get; private set; }

        public DateTime? CanceledAt { get; private set; }

        public static Payment Create(Guid orderId, Guid userId, decimal amount)
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                UserId = userId,
                Amount = amount,
                Status = PaymentStatus.Authorized,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AuthorizedAt = DateTime.UtcNow
            };
        }

        public void Capture()
        {
            Status = PaymentStatus.Captured;
            UpdatedAt = DateTime.UtcNow;
            CapturedAt = DateTime.UtcNow;
        }

        public void CanceleAuthorization()
        {
            Status = PaymentStatus.AuthorizationCanceled;
            UpdatedAt = DateTime.UtcNow;
            CanceledAt = DateTime.UtcNow;
        }
    }
}
