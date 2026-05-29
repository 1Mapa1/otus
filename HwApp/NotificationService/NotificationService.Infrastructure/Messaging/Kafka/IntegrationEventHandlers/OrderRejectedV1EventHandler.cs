using MediatR;
using NotificationService.Application.Notifications.CreateOrderRejectedNotification;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal sealed class OrderRejectedV1EventHandler
        : KafkaIntegrationEventHandler<OrderRejectedV1Event>
    {
        public OrderRejectedV1EventHandler(ISender sender)
            : base(sender)
        {
        }

        public override string EventType => "order.rejected.v1";

        protected override Task HandleAsync(
            OrderRejectedV1Event integrationEvent,
            CancellationToken cancellationToken)
        {
            return _sender.Send(
                new CreateOrderRejectedNotificationCommand(
                    integrationEvent.OrderId,
                    integrationEvent.UserId,
                    integrationEvent.Price,
                    integrationEvent.FailureReason,
                    null),
                cancellationToken);
        }
    }
}
