using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Reservations.CreateReservation
{
    public sealed record CreateReservationCommand(
        Guid OrderId,
        Guid UserId,
        Guid DeliverySlotId) : IRequest<Result<CreateReservationResult>>;
}
