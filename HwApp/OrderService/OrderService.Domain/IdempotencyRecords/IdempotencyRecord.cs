namespace OrderService.Domain.IdempotencyRecords
{
    public sealed class IdempotencyRecord
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid IdempotencyKey { get; private set; }

        public string RequestHash { get; private set; } = string.Empty;

        public IdempotencyRecordStatus Status { get; private set; }

        public Guid? OrderId { get; private set; }

        public string? ResponseBody { get; private set; }

        public DateTime? LockedUntil { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public DateTime ExpiresAt { get; private set; }

        private IdempotencyRecord() { }

        public static IdempotencyRecord Create(Guid userId, Guid idempotencyKey, string requestHash, TimeSpan processingLockTtl, TimeSpan? expiresTtl = null)
        {
            var now = DateTime.UtcNow;
            return new IdempotencyRecord
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IdempotencyKey = idempotencyKey,
                RequestHash = requestHash,
                Status = IdempotencyRecordStatus.Processing,
                CreatedAt = now,
                UpdatedAt = now,
                LockedUntil = now.Add(processingLockTtl),
                ExpiresAt = expiresTtl is null ? now.AddDays(1) : now.Add(expiresTtl.Value)
            };
        }

        public void MarkAsCompleted(Guid? orderId, string responseBody)
        {
            Status = IdempotencyRecordStatus.Completed;
            OrderId = orderId;
            ResponseBody = responseBody;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
