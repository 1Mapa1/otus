using MediatR;
using NotificationService.Application.Notifications.CreateOrderPaidNotification;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal class OrderPaidEventHandler
        : KafkaIntegrationEventHandler<OrderPaidEvent>
    {
        public OrderPaidEventHandler(ISender sender)
            : base(sender)
        {
        }

        public override string EventType => "order.paid.v1";

        protected override Task HandleAsync(
            OrderPaidEvent integrationEvent,
            CancellationToken cancellationToken)
        {
            return _sender.Send(
                new CreateOrderPaidNotificationCommand(
                    integrationEvent.UserId,
                    integrationEvent.OrderId,
                    integrationEvent.Price),
                cancellationToken);
        }
    }
}
