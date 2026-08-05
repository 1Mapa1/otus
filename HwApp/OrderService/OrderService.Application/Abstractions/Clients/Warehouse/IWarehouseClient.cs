using OrderService.Application.Abstractions.Clients.Warehouse.CancelReservation;
using OrderService.Application.Abstractions.Clients.Warehouse.CreateReservation;

namespace OrderService.Application.Abstractions.Clients.Warehouse
{
    public interface IWarehouseClient
    {
        Task<CancelReservationResult> CancelReservationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<CreateReservationResult> CreateReservationAsync(
            Guid orderId,
            Guid userId,
            IReadOnlyList<CreateReservationItem> products,
            CancellationToken cancellationToken = default);
    }
}
