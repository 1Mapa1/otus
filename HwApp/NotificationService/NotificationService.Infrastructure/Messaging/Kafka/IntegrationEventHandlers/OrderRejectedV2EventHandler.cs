using MediatR;
using NotificationService.Application.Notifications.CreateOrderRejectedNotification;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal sealed class OrderRejectedV2EventHandler
        : KafkaIntegrationEventHandler<OrderRejectedV2Event>
    {
        public OrderRejectedV2EventHandler(ISender sender)
            : base(sender)
        {
        }

        public override string EventType => "order.rejected.v2";

        protected override Task HandleAsync(
            OrderRejectedV2Event integrationEvent,
            CancellationToken cancellationToken)
        {
            return _sender.Send(
                new CreateOrderRejectedNotificationCommand(
                    integrationEvent.OrderId,
                    integrationEvent.UserId,
                    integrationEvent.TotalAmount,
                    integrationEvent.FailureReason,
                    integrationEvent.FailureDetails),
                cancellationToken);
        }
    }
}
