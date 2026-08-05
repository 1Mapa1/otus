using CatalogService.Infrastructure.Messaging.Kafka;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CatalogService.Infrastructure.Workers
{
    internal sealed class OutboxPublisher : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OutboxPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var isSent = false;

                try
                {
                    isSent = await PublishAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Outbox publisher failed: " + ex);
                }

                if (!isSent)
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task<bool> PublishAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();

            var databaseContext = scope.ServiceProvider.GetRequiredService<CatalogWriteDbContext>();
            var kafkaProducer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();

            await using var transaction = await databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var messages = await databaseContext.OutboxMessages
                .FromSqlRaw("""
                    SELECT *
                    FROM outbox_messages
                    WHERE published_at IS NULL
                    ORDER BY created_at
                    LIMIT 50
                    FOR UPDATE SKIP LOCKED
                    """)
                .ToListAsync(cancellationToken);

            if (messages.Count == 0)
                return false;

            foreach (var message in messages)
            {
                await kafkaProducer.ProduceAsync(
                    message.Topic,
                    message.Key,
                    message.Payload,
                    cancellationToken);

                message.PublishedAt = DateTime.UtcNow;
            }

            await databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
    }
}
