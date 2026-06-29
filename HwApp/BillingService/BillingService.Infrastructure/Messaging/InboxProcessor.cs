using BillingService.Infrastructure.Messaging.Contracts;
using BillingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infrastructure.Messaging
{
    internal sealed class InboxProcessor
    {
        private readonly DatabaseContext _dbContext;
        private readonly IntegrationEventDispatcher _dispatcher;

        public InboxProcessor(
            DatabaseContext dbContext,
            IntegrationEventDispatcher dispatcher)
        {
            _dbContext = dbContext;
            _dispatcher = dispatcher;
        }

        /// <returns><c>true</c> if message was processed; <c>false</c> if duplicate.</returns>
        public async Task<bool> ProcessAsync(
            IntegrationEventEnvelope message,
            string kafkaTopic,
            int kafkaPartition,
            long kafkaOffset,
            CancellationToken cancellationToken)
        {
            if (!_dispatcher.CanHandle(message.EventType))
            {
                throw new DeadLetterMessageException(
                    $"No handler registered for '{message.EventType}'.");
            }

            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            var isNewMessage = await TryRegisterInboxMessageAsync(
                message,
                kafkaTopic,
                kafkaPartition,
                kafkaOffset,
                cancellationToken);

            if (!isNewMessage)
            {
                await transaction.CommitAsync(cancellationToken);

                return false;
            }

            await _dispatcher.DispatchAsync(message, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }

        private async Task<bool> TryRegisterInboxMessageAsync(
            IntegrationEventEnvelope message,
            string kafkaTopic,
            int kafkaPartition,
            long kafkaOffset,
            CancellationToken cancellationToken)
        {
            var affectedRows = await _dbContext.Database
                .ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO inbox_messages (
                        event_id,
                        event_type,
                        occurred_at_utc,
                        processed_at_utc,
                        kafka_topic,
                        kafka_partition,
                        kafka_offset
                    )
                    VALUES (
                        {message.EventId},
                        {message.EventType},
                        {message.OccurredAt},
                        {DateTime.UtcNow},
                        {kafkaTopic},
                        {kafkaPartition},
                        {kafkaOffset}
                    )
                    ON CONFLICT (event_id) DO NOTHING;
                    """,
                    cancellationToken);

            return affectedRows == 1;
        }
    }
}
