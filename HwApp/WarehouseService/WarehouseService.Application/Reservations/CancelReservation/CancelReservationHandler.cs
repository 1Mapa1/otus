using MediatR;
using WarehouseService.Application.Common;
using WarehouseService.Application.Reservations.Operations;

namespace WarehouseService.Application.Reservations.CancelReservation
{
    internal sealed class CancelReservationHandler : IRequestHandler<CancelReservationCommand, Result>
    {
        private static readonly Error UnknownError = new("UnknownError", "An unknown error occurred while canceling the reservation.", ErrorType.Failure);

        private readonly IReservationRepository _reservationRepository;

        public CancelReservationHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var result = await _reservationRepository.CancelAsync(request.OrderId, cancellationToken);

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
