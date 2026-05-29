using MediatR;
using NotificationService.Application.Notifications.CreateOrderPaidNotification;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal class OrderConfirmedEventHandler 
        : KafkaIntegrationEventHandler<OrderConfirmedEvent>
    {
        public OrderConfirmedEventHandler(ISender sender)
            : base(sender)
        {
        }

        public override string EventType => "order.confirmed.v1";

        protected override Task HandleAsync(
            OrderConfirmedEvent integrationEvent,
            CancellationToken cancellationToken)
        {
            return _sender.Send(
                new CreateOrderPaidNotificationCommand(
                    integrationEvent.OrderId,
                    integrationEvent.UserId,
                    integrationEvent.TotalAmount),
                cancellationToken);
        }
    }
}
