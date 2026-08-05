using OrderService.Application.Abstractions.Clients.Delivery.CancelReservation;
using OrderService.Application.Abstractions.Clients.Delivery.CreateReservation;
using OrderService.Domain.Orders;

namespace OrderService.Application.Abstractions.Clients.Delivery
{
    public interface IDeliveryClient
    {
        Task<CreateReservationResult> CreateReservationAsync(
            Guid orderId,
            Guid customerId,
            Guid deliverySlotId,
            DeliveryAddressSnapshot deliveryAddress,
            CancellationToken cancellationToken = default);

        Task<CancelReservationResult> CancelReservationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
