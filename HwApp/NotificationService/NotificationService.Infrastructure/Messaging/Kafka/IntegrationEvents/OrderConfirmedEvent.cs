namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents
{
    internal sealed record OrderConfirmedEvent(
        Guid OrderId,
        Guid UserId,
        decimal TotalAmount);
}
