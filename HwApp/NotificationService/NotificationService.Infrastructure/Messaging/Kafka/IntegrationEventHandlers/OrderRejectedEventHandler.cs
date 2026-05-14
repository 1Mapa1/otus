using MediatR;
using NotificationService.Application.Notifications.CreateOrderRejectedNotification;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEvents;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal sealed class OrderRejectedEventHandler
        : KafkaIntegrationEventHandler<OrderRejectedEvent>
    {
        public OrderRejectedEventHandler(ISender sender)
            : base(sender)
        {
        }

        public override string EventType => "order.rejected.v1";

        protected override Task HandleAsync(
            OrderRejectedEvent integrationEvent,
            CancellationToken cancellationToken)
        {
            return _sender.Send(
                new CreateOrderRejectedNotificationCommand(
                    integrationEvent.OrderId,
                    integrationEvent.UserId,
                    integrationEvent.Price,
                    integrationEvent.FailureReason),
                cancellationToken);
        }
    }
}
