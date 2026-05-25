using MediatR;
using NotificationService.Domain.Customers;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.GetNotificationById
{
    public sealed class GetNotificationByIdHandler : IRequestHandler<GetNotificationByIdQuery, NotificationDetailsDto?>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationCustomerRepository _customerRepository;

        public GetNotificationByIdHandler(
            INotificationRepository notificationRepository,
            INotificationCustomerRepository customerRepository)
        {
            _notificationRepository = notificationRepository;
            _customerRepository = customerRepository;
        }

        public async Task<NotificationDetailsDto?> Handle(GetNotificationByIdQuery request, CancellationToken ct)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, ct);

            if (notification == null)
                return null;

            if (notification.UserId != request.UserId)
                return null;

            var customer = await _customerRepository.GetByIdAsync(notification.UserId, ct);

            return new NotificationDetailsDto(
                notification.Id,
                customer?.Name,
                customer?.Email,
                notification.OrderId,
                notification.Type.ToString(),
                notification.Subject,
                notification.Body,
                notification.CreatedAt);
        }
    }
}
