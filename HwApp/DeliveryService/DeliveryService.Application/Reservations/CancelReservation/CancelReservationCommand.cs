using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Reservations.CancelReservation
{
    public sealed record CancelReservationCommand(
        Guid OrderId) : IRequest<Result>;
}
