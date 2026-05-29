using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Reservations.CancelReservation
{
    public sealed record CancelReservationCommand(Guid OrderId) : IRequest<Result>;
}
