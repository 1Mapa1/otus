using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderService.Application.Abstractions.Persistence;
using OrderService.Domain.IdempotencyRecords;

namespace OrderService.Infrastructure.Persistence.Repositories
{
    internal class IdempotencyRepository : IIdempotencyRepository
    {
        private readonly DatabaseContext _databaseContext;

        public IdempotencyRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IdempotencyRecord> GetRecordAsync(Guid userId, Guid idempotencyKey, CancellationToken cancellationToken)
        {
            return await _databaseContext.IdempotencyRecords
                .SingleAsync(
                    x => x.UserId == userId &&
                         x.IdempotencyKey == idempotencyKey,
                    cancellationToken);
        }

        public async Task<bool> TryCreateRecordAsync(IdempotencyRecord record, CancellationToken cancellationToken)
        {
            try
            {
                await _databaseContext.IdempotencyRecords.AddAsync(record, cancellationToken);
                await _databaseContext.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException postgresException &&
                postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                _databaseContext.Entry(record).State = EntityState.Detached;
                return false;
            }
        }

        public async Task<bool> TryLockRecordAsync(Guid userId, Guid idempotencyKey, TimeSpan processingLockTtl, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var newLockedUntil = now.Add(processingLockTtl);

            var affectedRows = await _databaseContext.IdempotencyRecords
                .Where(x =>
                    x.UserId == userId &&
                    x.IdempotencyKey == idempotencyKey &&
                    x.Status == IdempotencyRecordStatus.Processing &&
                    x.LockedUntil <= now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.LockedUntil, newLockedUntil)
                    .SetProperty(x => x.UpdatedAt, now),
                    cancellationToken);

            return affectedRows == 1;
        }
    }
}
