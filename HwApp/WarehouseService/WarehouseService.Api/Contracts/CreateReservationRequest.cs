using WarehouseService.Application.Reservations.CreateReservation;

namespace WarehouseService.Api.Contracts
{
    public sealed record CreateReservationRequest(
        Guid OrderId,
        Guid UserId,
        List<CreateReservationItem> Items);
}
