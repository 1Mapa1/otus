using DeliveryService.Application.Reservations.Operations;

namespace DeliveryService.Application.Reservations
{
    public interface IDeliveryReservationRepository
    {
        Task<CancelReservationOperationResult> CancelAsync(Guid orderId, CancellationToken cancellationToken);
        Task<ReserveDeliverySlotOperationResult> ReserveAsync(Guid orderId, Guid userId, Guid deliverySlotId, CancellationToken cancellationToken);
    }
}
