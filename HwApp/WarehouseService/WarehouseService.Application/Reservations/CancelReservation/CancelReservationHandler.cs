using MediatR;
using WarehouseService.Application.Common;
using WarehouseService.Application.Reservations.Operations;

namespace WarehouseService.Application.Reservations.CancelReservation
{
    public sealed class CancelReservationHandler : IRequestHandler<CancelReservationCommand, Result>
    {
        private static readonly Error UnknownError = new("UnknownError", "An unknown error occurred while creating the reservation.", ErrorType.Failure);
        private static readonly Error StockStateConflict = new("StockStateConflict", "The stock state is in conflict.", ErrorType.Conflict);
        private static readonly Error InvalidReservationState = new("InvalidReservationState", "The reservation is in an invalid state.", ErrorType.Conflict);

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
                case CancelReservationOperationResult.InvalidReservationState:
                    return Result.Failure(InvalidReservationState);
                case CancelReservationOperationResult.StockStateConflict:
                    return Result.Failure(StockStateConflict);
                default:
                    return Result.Failure(UnknownError);
            }
        }
    }
}
