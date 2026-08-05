using BillingService.Infrastructure.Messaging.Contracts;

namespace BillingService.Infrastructure.Messaging.Handlers
{
    internal interface IIntegrationEventHandler
    {
        string EventType { get; }

        Task HandleAsync(
            IntegrationEventEnvelope message,
            CancellationToken cancellationToken);
    }
}
