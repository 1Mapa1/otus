using OrderService.Domain.IdempotencyRecords;

namespace OrderService.Application.Idempotency
{
    public interface IIdempotencyService
    {
        Task<IdempotencyStartResult<TResponse>> StartAsync<TRequest, TResponse>(Guid userId, Guid idempotencyKey, TRequest request, CancellationToken cancellationToken);

        public void Complete<TResponse>(IdempotencyRecord record, Guid? orderId, TResponse response);
    }
}
