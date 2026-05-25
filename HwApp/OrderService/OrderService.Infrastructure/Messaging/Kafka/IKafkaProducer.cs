namespace OrderService.Infrastructure.Messaging.Kafka
{
    internal interface IKafkaProducer
    {
        Task ProduceAsync(
            string topic,
            string key,
            string value,
            CancellationToken cancellationToken = default);
    }
}
