using OrderService.Application.Abstractions.Delivery.CancelReservation;
using OrderService.Application.Abstractions.Delivery.CreateReservation;

namespace OrderService.Application.Abstractions.Delivery
{
    public interface IDeliveryClient
    {
        Task<CreateReservationResult> CreateReservationAsync(
            Guid orderId,
            Guid userId,
            Guid deliverySlotId,
            CancellationToken cancellationToken = default);

        Task<CancelReservationResult> CancelReservationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
