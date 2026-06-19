namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents
{
    internal sealed record OrderRejectedV2Event(
        Guid OrderId,
        Guid UserId,
        decimal TotalAmount,
        string FailureReason,
        string? FailureDetails);
}
