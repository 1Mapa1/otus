using DeliveryService.Application.Common;
using DeliveryService.Application.Reservations.Operations;
using MediatR;

namespace DeliveryService.Application.Reservations.CancelReservation
{
    internal sealed class CancelReservationHandler : IRequestHandler<CancelReservationCommand, Result>
    {
        private static readonly Error SlotStateConflict = new(
            "SlotStateConflict",
            "Delivery slot state conflict occurred while canceling the reservation.",
            ErrorType.Conflict);

        private readonly IDeliveryReservationRepository _deliveryReservationRepository;

        public CancelReservationHandler(IDeliveryReservationRepository deliveryReservationRepository)
        {
            _deliveryReservationRepository = deliveryReservationRepository;
        }

        public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            CancelReservationOperationResult result = await _deliveryReservationRepository.CancelAsync(
                request.OrderId,
                cancellationToken);

            return result switch
            {
                CancelReservationOperationResult.Success or CancelReservationOperationResult.ReservationNotFound =>
                    Result.Success(),

                CancelReservationOperationResult.SlotStateConflict =>
                    Result.Failure(SlotStateConflict),

                _ => throw new InvalidOperationException(
                    $"Unknown cancel reservation operation status: {result}")
            };
        }
    }
}
