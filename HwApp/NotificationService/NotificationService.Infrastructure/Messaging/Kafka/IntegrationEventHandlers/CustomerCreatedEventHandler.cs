using MediatR;
using NotificationService.Application.Customers.UpsertNotificationCustomer;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal sealed class CustomerCreatedEventHandler 
        : KafkaIntegrationEventHandler<CustomerCreatedEvent>
    {
        public CustomerCreatedEventHandler(ISender sender)
            : base(sender)
        {
        }

        public override string EventType => "customer.created.v1";

        protected override Task HandleAsync(
            CustomerCreatedEvent integrationEvent,
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
