namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents
{
    public sealed record OrderRejectedV1Event(
        Guid OrderId,
        Guid UserId,
        decimal Price,
        string FailureReason);
}
