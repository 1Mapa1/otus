using Microsoft.Extensions.Options;
using OrderService.Application.Abstractions.Idempotency;
using OrderService.Application.Abstractions.Persistence;
using OrderService.Domain.IdempotencyRecords;
using System.Text.Json;

namespace OrderService.Application.Idempotency
{
    internal class IdempotencyService : IIdempotencyService
    {
        private readonly IRequestHashCalculator _requestHashCalculator;
        private readonly IIdempotencyRepository _idempotencyRepository;
        private readonly IdempotencyOptions _options;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public IdempotencyService(IRequestHashCalculator requestHashCalculator, IIdempotencyRepository idempotencyRepository, IOptions<IdempotencyOptions> options)
        {
            _requestHashCalculator = requestHashCalculator;
            _idempotencyRepository = idempotencyRepository;
            _options = options.Value;
        }

        public async Task<IdempotencyStartResult<TResponse>> StartAsync<TRequest, TResponse>(Guid userId, Guid idempotencyKey, TRequest request, CancellationToken cancellationToken)
        {
            var requestHash = _requestHashCalculator.Calculate(request);

            var record = IdempotencyRecord.Create(userId, idempotencyKey, requestHash, _options.ProcessingLockTtl, _options.RecordTtl);

            var isCreated = await _idempotencyRepository.TryCreateRecordAsync(record, cancellationToken);

            if (isCreated)
                return IdempotencyStartResult<TResponse>.Success(record);

            var existingRecord = await _idempotencyRepository.GetRecordAsync(userId, idempotencyKey, cancellationToken);

            if (existingRecord.RequestHash != requestHash)
                return IdempotencyStartResult<TResponse>.Conflict();

            if (existingRecord.Status == IdempotencyRecordStatus.Completed)
            {
                var savedResult = JsonSerializer.Deserialize<TResponse>(existingRecord.ResponseBody!, JsonOptions);

                return IdempotencyStartResult<TResponse>.Completed(savedResult);
            }

            if (existingRecord.LockedUntil > DateTime.UtcNow)
                return IdempotencyStartResult<TResponse>.AlreadyProcessing();

            var canLock = await _idempotencyRepository.TryLockRecordAsync(userId, idempotencyKey, _options.ProcessingLockTtl, cancellationToken);

            if (!canLock)
                return IdempotencyStartResult<TResponse>.AlreadyProcessing();

            var lockedRecord = await _idempotencyRepository.GetRecordAsync(userId, idempotencyKey, cancellationToken);
            return IdempotencyStartResult<TResponse>.Success(lockedRecord);
        }

        public void Complete<TResponse>(IdempotencyRecord record, Guid? orderId, TResponse response)
        {
            var responseBody = JsonSerializer.Serialize(response, JsonOptions);

            record.MarkAsCompleted(
                orderId,
                responseBody);
        }
    }
}
