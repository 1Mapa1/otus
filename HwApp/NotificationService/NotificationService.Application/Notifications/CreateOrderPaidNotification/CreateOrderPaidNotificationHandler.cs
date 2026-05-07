using MediatR;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.CreateOrderPaidNotification
{
    public sealed class CreateOrderPaidNotificationHandler : IRequestHandler<CreateOrderPaidNotificationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;

        public CreateOrderPaidNotificationHandler(IUnitOfWork unitOfWork, INotificationRepository notificationRepository)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(CreateOrderPaidNotificationCommand request, CancellationToken ct)
        {
            var notification = Notification.CreateOrderPaid(
                request.UserId,
                request.OrderId,
                request.Price);

            await _notificationRepository.AddAsync(notification, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
