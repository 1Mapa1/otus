using MediatR;

namespace NotificationService.Application.Notifications.CreateOrderConfirmedNotification
{
    public sealed record CreateOrderConfirmedNotificationCommand(
        Guid OrderId,
        Guid UserId,
        decimal TotalAmount) : IRequest;
}
