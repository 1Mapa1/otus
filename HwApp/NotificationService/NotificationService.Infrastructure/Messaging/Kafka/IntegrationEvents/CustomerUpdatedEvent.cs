namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents
{
    public sealed record CustomerUpdatedEvent(
        Guid Id,
        string Name,
        string Email);
}
