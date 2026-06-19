using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Reservations.CreateReservation
{
    public sealed record CreateReservationCommand(
        Guid OrderId,
        Guid UserId,
        List<CreateReservationItem> Items) : IRequest<Result<CreateReservationResult>>;
}
