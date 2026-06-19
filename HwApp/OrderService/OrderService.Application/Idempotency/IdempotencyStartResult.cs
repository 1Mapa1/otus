using OrderService.Domain.IdempotencyRecords;

namespace OrderService.Application.Idempotency
{
    public sealed record IdempotencyStartResult<TResponse>
    {
        public bool IsConflict { get; private set; }
        public bool IsAlreadyProcessing { get; private set; }
        public bool IsCompleted { get; private set; }

        public TResponse? SavedCreateOrderResult { get; private set; }

        public IdempotencyRecord? Record { get; private set; } = null;

        public static IdempotencyStartResult<TResponse> Conflict() => new() { IsConflict = true };

        public static IdempotencyStartResult<TResponse> AlreadyProcessing() => new() { IsAlreadyProcessing = true };

        public static IdempotencyStartResult<TResponse> Completed(TResponse? savedResult) => new() { IsCompleted = true, SavedCreateOrderResult = savedResult };

        public static IdempotencyStartResult<TResponse> Success(IdempotencyRecord idempotencyRecord) => new() { Record = idempotencyRecord };
    }
}
