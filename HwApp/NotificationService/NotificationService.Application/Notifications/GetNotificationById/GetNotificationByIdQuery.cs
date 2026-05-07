using MediatR;

namespace NotificationService.Application.Notifications.GetNotificationById
{
    public sealed record GetNotificationByIdQuery(
        Guid UserId,
        Guid NotificationId) : IRequest<NotificationDetailsDto?>;
}
