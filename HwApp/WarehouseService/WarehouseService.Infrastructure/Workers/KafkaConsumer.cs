using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WarehouseService.Infrastructure.Messaging;
using WarehouseService.Infrastructure.Messaging.Kafka;

namespace WarehouseService.Infrastructure.Workers
{
    internal sealed class KafkaConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly KafkaOptions _options;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(
            IServiceScopeFactory scopeFactory,
            IOptions<KafkaOptions> options,
            ILogger<KafkaConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeLoopAsync(stoppingToken), stoppingToken);
        }

        private async Task ConsumeLoopAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = _options.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false,
                EnablePartitionEof = false
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogError(
                        "Kafka consumer error. Code: {Code}, Reason: {Reason}, IsFatal: {IsFatal}",
                        error.Code,
                        error.Reason,
                        error.IsFatal);
                })
                .Build();

            consumer.Subscribe(_options.Topics);

            _logger.LogInformation(
                "Warehouse Kafka consumer started. GroupId: {GroupId}. Topics: {Topics}",
                _options.GroupId,
                string.Join(", ", _options.Topics));

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<Ignore, string>? consumeResult = null;

                    try
                    {
                        consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));

                        if (consumeResult?.Message is null)
                            continue;

                        using var scope = _scopeFactory.CreateScope();
                        var processor = scope.ServiceProvider
                            .GetRequiredService<ProductLifecycleProcessor>();

                        await processor.ProcessAsync(
                            consumeResult.Message.Value,
                            stoppingToken);

                        consumer.StoreOffset(consumeResult);
                        consumer.Commit(consumeResult);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Failed to process Kafka message. Offset will not be committed.");
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
