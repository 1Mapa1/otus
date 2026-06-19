using WarehouseService.Application.Reservations.Operations;

namespace WarehouseService.Application.Reservations
{
    public interface IReservationRepository
    {
        Task<CancelReservationOperationResult> CancelAsync(Guid orderId, CancellationToken cancellationToken);
        Task<ReserveProductsOperationResult> ReserveAsync(Guid orderId, Guid userId, List<ReserveProductItem> reserveProductItems, CancellationToken cancellationToken);
    }
}
