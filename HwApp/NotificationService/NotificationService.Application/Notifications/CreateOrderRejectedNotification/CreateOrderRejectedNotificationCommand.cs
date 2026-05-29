using MediatR;

namespace NotificationService.Application.Notifications.CreateOrderRejectedNotification
{
    public sealed record CreateOrderRejectedNotificationCommand(
        Guid OrderId,
        Guid UserId,
        decimal TotalAmount,
        string FailureReason,
        string? FailureDetails) : IRequest;
}
