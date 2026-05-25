namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents
{
    public sealed record OrderRejectedEvent(
        Guid OrderId,
        Guid UserId,
        decimal Price,
        string FailureReason);
}
