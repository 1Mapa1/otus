using MediatR;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.CreateOrderConfirmedNotification
{
    public sealed class CreateOrderConfirmedNotificationHandler : IRequestHandler<CreateOrderConfirmedNotificationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;

        public CreateOrderConfirmedNotificationHandler(IUnitOfWork unitOfWork, INotificationRepository notificationRepository)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(CreateOrderConfirmedNotificationCommand request, CancellationToken ct)
        {
            var notification = Notification.CreateOrderConfirmed(
                request.UserId,
                request.OrderId,
                request.TotalAmount);

            await _notificationRepository.AddAsync(notification, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
