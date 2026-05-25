using MediatR;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.CreateOrderRejectedNotification
{
    public sealed class CreateOrderRejectedNotificationHandler : IRequestHandler<CreateOrderRejectedNotificationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;

        public CreateOrderRejectedNotificationHandler(IUnitOfWork unitOfWork, INotificationRepository notificationRepository)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(CreateOrderRejectedNotificationCommand request, CancellationToken ct)
        {
            var notification = Notification.CreateOrderRejected(
                 request.UserId,
                 request.OrderId,
                 request.Price,
                 request.FailureReason);

            await _notificationRepository.AddAsync(notification, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
