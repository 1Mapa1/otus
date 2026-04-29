using CustomerService.Infrastructure.Messaging.Kafka;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CustomerService.Infrastructure.Workers
{
    internal class OutboxPublisher : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OutboxPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var isSend = false;

                try
                {
                    isSend = await PublishAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Outbox publisher failed: " + ex);
                }

                if (!isSend)
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task<bool> PublishAsync(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var kafka = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();

            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            var messages = await db.OutboxMessages
                .FromSqlRaw("""
                    SELECT *
                    FROM outbox_messages
                    WHERE published_at IS NULL
                    ORDER BY created_at
                    LIMIT 50
                    FOR UPDATE SKIP LOCKED
                """)
                .ToListAsync(ct);

            if (messages.Count == 0)
                return false;

            foreach (var message in messages)
            {
                await kafka.ProduceAsync(
                    message.Topic,
                    message.Id.ToString(),
                    message.Payload,
                    ct);

                message.PublishedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return true;
        }
    }
}
