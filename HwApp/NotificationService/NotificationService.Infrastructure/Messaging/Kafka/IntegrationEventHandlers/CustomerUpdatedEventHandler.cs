using MediatR;
using NotificationService.Application.Customers.UpsertNotificationCustomer;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal sealed class CustomerUpdatedEventHandler
        : KafkaIntegrationEventHandler<CustomerUpdatedEvent>
    {
        public CustomerUpdatedEventHandler(ISender sender)
            : base(sender)
        {
        }

        public override string EventType => "customer.updated.v1";

        protected override Task HandleAsync(
            CustomerUpdatedEvent integrationEvent,
            CancellationToken cancellationToken)
        {
            return _sender.Send(
                new UpsertNotificationCustomerCommand(
                    integrationEvent.Id,
                    integrationEvent.Name,
                    integrationEvent.Email),
                cancellationToken);
        }
    }
}
