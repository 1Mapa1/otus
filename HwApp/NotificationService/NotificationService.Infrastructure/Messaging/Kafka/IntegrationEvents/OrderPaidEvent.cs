namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents
{
    public sealed record OrderPaidEvent(
        Guid OrderId,
        Guid UserId,
        decimal Price);
}
