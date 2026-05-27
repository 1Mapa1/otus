using DeliveryService.Application.Common;
using DeliveryService.Application.Reservations.Operations;
using MediatR;

namespace DeliveryService.Application.Reservations.CancelReservation
{
    internal sealed class CancelReservationHandler : IRequestHandler<CancelReservationCommand, Result>
    {
        private static readonly Error UnknownError = new("UnknownError", "An unknown error occurred while canceling the reservation.", ErrorType.Failure);

        private readonly IDeliveryReservationRepository _deliveryReservationRepository;

        public CancelReservationHandler(IDeliveryReservationRepository deliveryReservationRepository)
        {
            _deliveryReservationRepository = deliveryReservationRepository;
        }

        public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            CancelReservationOperationResult result = await _deliveryReservationRepository.CancelAsync(request.OrderId, cancellationToken);

            switch (result)
            {
                case CancelReservationOperationResult.Success:
                case CancelReservationOperationResult.ReservationNotFound:
                    return Result.Success();

                default:
                    return Result.Failure(UnknownError);
            }
        }
    }
}
