using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Infrastructure.Messaging.Kafka;
using NotificationService.Infrastructure.Messaging.Kafka.HealthCheck;

namespace NotificationService.Infrastructure.Workers
{
    internal class KafkaConsumer : BackgroundService
    {
        private readonly KafkaOptions _options;
        private readonly KafkaConsumerState _state;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(
            IOptions<KafkaOptions> options,
            KafkaConsumerState state,
            IServiceScopeFactory scopeFactory,
            ILogger<KafkaConsumer> logger)
        {
            _options = options.Value;
            _state = state;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);
        }

        private void ConsumeLoop(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = _options.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnablePartitionEof = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, error) =>
                {
                    _state.MarkError(error.Reason);

                    _logger.LogError(
                        "Kafka consumer error. Code: {Code}, Reason: {Reason}, IsFatal: {IsFatal}",
                        error.Code,
                        error.Reason,
                        error.IsFatal);
                })
                .Build();

            _state.MarkStarted();

            consumer.Subscribe(_options.Topics);

            _state.MarkSubscribed();

            _logger.LogInformation(
                "Notification Kafka consumer started. GroupId: {GroupId}. Topics: {Topics}",
                _options.GroupId,
                string.Join(", ", _options.Topics));

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<string, string>? consumeResult = null;

                    try
                    {
                        consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));

                        _state.MarkPoll();

                        if (consumeResult?.Message is null)
                        {
                            continue;
                        }

                        using var scope = _scopeFactory.CreateScope();

                        var dispatcher = scope.ServiceProvider
                            .GetRequiredService<KafkaMessageDispatcher>();

                        dispatcher
                            .DispatchAsync(consumeResult.Message.Value, stoppingToken)
                            .GetAwaiter()
                            .GetResult();

                        consumer.Commit(consumeResult);

                        _logger.LogInformation(
                            "Kafka message processed. Topic: {Topic}, Key: {Key}, Partition: {Partition}, Offset: {Offset}",
                            consumeResult.Topic,
                            consumeResult.Message.Key,
                            consumeResult.Partition.Value,
                            consumeResult.Offset.Value);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ConsumeException ex)
                    {
                        _state.MarkError(ex);

                        _logger.LogError(
                            ex,
                            "Kafka consume error. Reason: {Reason}",
                            ex.Error.Reason);
                    }
                    catch (KafkaException ex)
                    {
                        _state.MarkError(ex);

                        _logger.LogError(
                            ex,
                            "Kafka error while processing message. Topic: {Topic}, Offset: {Offset}",
                            consumeResult?.Topic,
                            consumeResult?.Offset.Value);
                    }
                    catch (Exception ex)
                    {
                        _state.MarkError(ex);

                        _logger.LogError(
                            ex,
                            "Kafka message processing failed. Topic: {Topic}, Key: {Key}, Partition: {Partition}, Offset: {Offset}",
                            consumeResult?.Topic,
                            consumeResult?.Message?.Key,
                            consumeResult?.Partition.Value,
                            consumeResult?.Offset.Value);
                    }
                }
            }
            finally
            {
                consumer.Close();

                _logger.LogInformation("Notification Kafka consumer stopped.");
            }
        }
    }
}
