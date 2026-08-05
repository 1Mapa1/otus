using System.Text.Json;
using Microsoft.Extensions.Options;

namespace NotificationService.Infrastructure.Messaging.Kafka
{
    internal sealed class KafkaDlqPublisher
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web);

        private readonly IKafkaProducer _producer;
        private readonly KafkaOptions _options;

        public KafkaDlqPublisher(
            IKafkaProducer producer,
            IOptions<KafkaOptions> options)
        {
            _producer = producer;
            _options = options.Value;
        }

        public Task PublishAsync(
            string originalMessage,
            string sourceTopic,
            int partition,
            long offset,
            string error,
            CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Serialize(
                new
                {
                    originalMessage,
                    sourceTopic,
                    partition,
                    offset,
                    error,
                    failedAt = DateTimeOffset.UtcNow
                },
                JsonOptions);

            var key = $"{sourceTopic}:{partition}:{offset}";

            return _producer.ProduceAsync(
                _options.DlqTopic,
                key,
                payload,
                cancellationToken);
        }
    }
}
