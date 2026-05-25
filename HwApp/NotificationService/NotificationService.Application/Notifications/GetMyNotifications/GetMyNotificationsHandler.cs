using MediatR;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.GetMyNotifications
{
    public sealed class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, IReadOnlyList<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetMyNotificationsHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IReadOnlyList<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(
                request.UserId,
                ct);

            return notifications
                .Select(x => new NotificationDto(
                    x.Id,
                    x.OrderId,
                    x.Type.ToString(),
                    x.Subject,
                    x.CreatedAt))
                .ToList();
        }
    }
}
