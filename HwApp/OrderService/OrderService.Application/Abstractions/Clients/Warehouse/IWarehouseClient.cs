using OrderService.Application.Abstractions.Clients.Warehouse.CancelReservation;
using OrderService.Application.Abstractions.Clients.Warehouse.CreateReservation;
using OrderService.Application.Abstractions.Clients.Warehouse.ResolveProducts;

namespace OrderService.Application.Abstractions.Clients.Warehouse
{
    public interface IWarehouseClient
    {
        public Task<ResolveProductsResult> ResolveProductsAsync(
            IReadOnlyList<ResolveProductItem> products,
            CancellationToken cancellationToken = default);

        public Task<CancelReservationResult> CancelReservationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        public Task<CreateReservationResult> CreateReservationAsync(
            Guid orderId,
            Guid userId,
            IReadOnlyList<CreateReservationItem> products,
            CancellationToken cancellationToken = default);
    }
}
