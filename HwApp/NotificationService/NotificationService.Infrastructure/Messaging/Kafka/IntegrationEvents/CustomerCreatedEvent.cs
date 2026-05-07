namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents
{
    public sealed record CustomerCreatedEvent(
        Guid Id,
        string Name,
        string Email);
}
