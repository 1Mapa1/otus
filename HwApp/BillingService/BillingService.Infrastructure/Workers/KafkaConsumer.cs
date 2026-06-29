using BillingService.Infrastructure.Messaging;
using BillingService.Infrastructure.Messaging.Contracts;
using BillingService.Infrastructure.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BillingService.Infrastructure.Workers
{
    internal sealed class KafkaConsumer : BackgroundService
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly KafkaOptions _options;
        private readonly KafkaDlqPublisher _dlqPublisher;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(
            IServiceScopeFactory scopeFactory,
            IOptions<KafkaOptions> options,
            KafkaDlqPublisher dlqPublisher,
            ILogger<KafkaConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _dlqPublisher = dlqPublisher;
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
                "Billing Kafka consumer started. GroupId: {GroupId}. Topics: {Topics}",
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
                        {
                            continue;
                        }

                        var messageValue = consumeResult.Message.Value;

                        IntegrationEventEnvelope envelope;

                        try
                        {
                            envelope = DeserializeEnvelope(messageValue);
                        }
                        catch (DeadLetterMessageException ex)
                        {
                            await SendToDlqAndCommitAsync(
                                consumer,
                                consumeResult,
                                messageValue,
                                ex.Message,
                                stoppingToken);

                            continue;
                        }

                        var disposition = await ProcessWithRetryAsync(
                            envelope,
                            consumeResult,
                            messageValue,
                            stoppingToken);

                        consumer.Commit(consumeResult);

                        LogDisposition(
                            disposition,
                            consumeResult,
                            envelope);
                    }
                    catch (OperationCanceledException)
                        when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(
                            ex,
                            "Kafka consume error: {Reason}",
                            ex.Error.Reason);
                    }
                }
            }
            finally
            {
                consumer.Close();

                _logger.LogInformation("Billing Kafka consumer stopped.");
            }
        }

        private enum MessageDisposition
        {
            Processed,
            Duplicate,
            DeadLetter
        }

        private async Task<MessageDisposition> ProcessWithRetryAsync(
            IntegrationEventEnvelope envelope,
            ConsumeResult<Ignore, string> consumeResult,
            string originalMessage,
            CancellationToken stoppingToken)
        {
            var attempt = 0;

            while (true)
            {
                attempt++;

                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();

                    var inboxProcessor = scope.ServiceProvider
                        .GetRequiredService<InboxProcessor>();

                    var isNewMessage = await inboxProcessor.ProcessAsync(
                        envelope,
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value,
                        stoppingToken);

                    if (!isNewMessage)
                    {
                        return MessageDisposition.Duplicate;
                    }

                    return MessageDisposition.Processed;
                }
                catch (DeadLetterMessageException ex)
                {
                    await SendToDlqAsync(
                        consumeResult,
                        originalMessage,
                        ex.Message,
                        stoppingToken);

                    _logger.LogWarning(
                        ex,
                        """
                        Kafka message sent to DLQ (non-retryable).
                        Topic: {Topic};
                        Partition: {Partition};
                        Offset: {Offset}
                        """,
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value);

                    return MessageDisposition.DeadLetter;
                }
                catch (Exception ex) when (attempt < _options.MaxRetryAttempts)
                {
                    _logger.LogWarning(
                        ex,
                        """
                        Kafka message processing failed, retrying.
                        Topic: {Topic};
                        Partition: {Partition};
                        Offset: {Offset};
                        Attempt: {Attempt}/{MaxAttempts}
                        """,
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value,
                        attempt,
                        _options.MaxRetryAttempts);

                    await Task.Delay(
                        TimeSpan.FromSeconds(_options.RetryDelaySeconds),
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    await SendToDlqAsync(
                        consumeResult,
                        originalMessage,
                        ex.Message,
                        stoppingToken);

                    _logger.LogError(
                        ex,
                        """
                        Kafka message sent to DLQ after retries.
                        Topic: {Topic};
                        Partition: {Partition};
                        Offset: {Offset}
                        """,
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value);

                    return MessageDisposition.DeadLetter;
                }
            }
        }

        private void LogDisposition(
            MessageDisposition disposition,
            ConsumeResult<Ignore, string> consumeResult,
            IntegrationEventEnvelope envelope)
        {
            switch (disposition)
            {
                case MessageDisposition.Processed:
                    _logger.LogInformation(
                        """
                        Kafka message processed.
                        Topic: {Topic};
                        Partition: {Partition};
                        Offset: {Offset};
                        EventId: {EventId};
                        EventType: {EventType}
                        """,
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value,
                        envelope.EventId,
                        envelope.EventType);
                    break;

                case MessageDisposition.Duplicate:
                    _logger.LogInformation(
                        """
                        Kafka message duplicate.
                        Topic: {Topic};
                        Partition: {Partition};
                        Offset: {Offset};
                        EventId: {EventId}
                        """,
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value,
                        envelope.EventId);
                    break;
            }
        }

        private async Task SendToDlqAndCommitAsync(
            IConsumer<Ignore, string> consumer,
            ConsumeResult<Ignore, string> consumeResult,
            string originalMessage,
            string error,
            CancellationToken stoppingToken)
        {
            await SendToDlqAsync(
                consumeResult,
                originalMessage,
                error,
                stoppingToken);

            consumer.Commit(consumeResult);

            _logger.LogWarning(
                """
                Kafka message sent to DLQ.
                Topic: {Topic};
                Partition: {Partition};
                Offset: {Offset};
                Error: {Error}
                """,
                consumeResult.Topic,
                consumeResult.Partition.Value,
                consumeResult.Offset.Value,
                error);
        }

        private Task SendToDlqAsync(
            ConsumeResult<Ignore, string> consumeResult,
            string originalMessage,
            string error,
            CancellationToken stoppingToken)
        {
            return _dlqPublisher.PublishAsync(
                originalMessage,
                consumeResult.Topic,
                consumeResult.Partition.Value,
                consumeResult.Offset.Value,
                error,
                stoppingToken);
        }

        private static IntegrationEventEnvelope DeserializeEnvelope(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new DeadLetterMessageException("Kafka message body is empty.");
            }

            try
            {
                var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope>(
                    json,
                    JsonOptions);

                if (envelope is null)
                {
                    throw new DeadLetterMessageException(
                        "Kafka message has invalid envelope.");
                }

                if (envelope.EventId == Guid.Empty)
                {
                    throw new DeadLetterMessageException(
                        "Kafka message envelope is missing EventId.");
                }

                if (string.IsNullOrWhiteSpace(envelope.EventType))
                {
                    throw new DeadLetterMessageException(
                        "Kafka message envelope is missing EventType.");
                }

                return envelope;
            }
            catch (JsonException ex)
            {
                throw new DeadLetterMessageException(
                    "Kafka message has invalid JSON envelope.",
                    ex);
            }
        }
    }
}
