using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CustomerService.Infrastructure.Messaging.Kafka
{
    internal sealed class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private bool _disposed;

        public KafkaProducer(IOptions<KafkaOptions> options)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                Acks = Enum.Parse<Acks>(options.Value.Acks),
                EnableIdempotence = options.Value.EnableIdempotence
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            try
            {
                await _producer.ProduceAsync(
                    topic,
                    new Message<string, string>
                    {
                        Key = key,
                        Value = value
                    },
                    cancellationToken);
            }
            catch (ProduceException<string, string> ex)
            {
                throw new InvalidOperationException(
                    $"Failed to publish Kafka message to topic '{topic}'. Reason: {ex.Error.Reason}",
                    ex);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();

            _disposed = true;
        }
    }
}
