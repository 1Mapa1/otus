using MediatR;

namespace NotificationService.Application.Notifications.GetMyNotifications
{
    public sealed record GetMyNotificationsQuery(Guid UserId) 
        : IRequest<IReadOnlyList<NotificationDto>>;
}
