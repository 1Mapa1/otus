using MediatR;

namespace NotificationService.Application.Notifications.CreateOrderPaidNotification
{
    public sealed record CreateOrderPaidNotificationCommand (
        Guid OrderId,
        Guid UserId,
        decimal Price) : IRequest;
}
