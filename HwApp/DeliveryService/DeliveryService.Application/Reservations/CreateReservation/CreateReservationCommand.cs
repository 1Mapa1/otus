using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Reservations.CreateReservation
{
    public sealed record CreateReservationCommand(
        Guid OrderId,
        Guid CustomerId,
        Guid DeliverySlotId,
        string City,
        string Street,
        string House,
        string? Apartment) : IRequest<Result<CreateReservationResult>>;
}
