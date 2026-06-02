namespace OrderService.Application.Idempotency
{
    internal sealed class IdempotencyOptions
    {
        public const string SectionName = "Idempotency";

        public TimeSpan ProcessingLockTtl { get; init; } = TimeSpan.FromSeconds(30);

        public TimeSpan RecordTtl { get; init; } = TimeSpan.FromHours(24);
    }
}
