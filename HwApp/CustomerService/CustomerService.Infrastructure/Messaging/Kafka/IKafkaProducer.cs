namespace CustomerService.Infrastructure.Messaging.Kafka
{
    internal interface IKafkaProducer
    {
        public Task ProduceAsync(
                string topic,
                string key,
                string value,
                CancellationToken cancellationToken = default
            );
    }
}
