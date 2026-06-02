using OrderService.Domain.IdempotencyRecords;

namespace OrderService.Application.Abstractions.Persistence
{
    public interface IIdempotencyRepository
    {
        Task<IdempotencyRecord> GetRecordAsync(Guid userId, Guid idempotencyKey, CancellationToken cancellationToken);
        Task<bool> TryCreateRecordAsync(IdempotencyRecord record, CancellationToken cancellationToken);
        Task<bool> TryLockRecordAsync(Guid userId, Guid idempotencyKey, TimeSpan processingLockTtl, CancellationToken cancellationToken);
    }
}
