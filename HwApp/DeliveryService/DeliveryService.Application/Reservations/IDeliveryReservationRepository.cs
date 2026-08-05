using DeliveryService.Application.Reservations.Operations;
using DeliveryService.Domain.Reservations;

namespace DeliveryService.Application.Reservations
{
    public interface IDeliveryReservationRepository
    {
        Task<CancelReservationOperationResult> CancelAsync(
            Guid orderId,
            CancellationToken cancellationToken);

        Task<ReserveDeliverySlotOperationResult> ReserveAsync(
            Guid orderId,
            Guid customerId,
            Guid deliverySlotId,
            DeliveryAddress address,
            CancellationToken cancellationToken);
    }
}
