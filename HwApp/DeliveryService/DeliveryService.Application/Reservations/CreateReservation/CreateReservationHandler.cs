using DeliveryService.Application.Common;
using DeliveryService.Application.Reservations.Operations;
using MediatR;

namespace DeliveryService.Application.Reservations.CreateReservation
{
    internal sealed class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Result<CreateReservationResult>>
    {
        private static readonly Error DeliverySlotUnavailable = new("DeliverySlotUnavailable", "The delivary slot unavailable", ErrorType.Conflict);
        private static readonly Error InvalidReservationState = new("InvalidReservationState", "The reservation is in an invalid state for this operation.", ErrorType.Conflict);
        private static readonly Error UnknownError = new("UnknownError", "An unknown error occurred while creating the reservation.", ErrorType.Failure);

        private readonly IDeliveryReservationRepository _deliveryReservationRepository;

        public CreateReservationHandler(IDeliveryReservationRepository deliveryReservationRepository)
        {
            _deliveryReservationRepository = deliveryReservationRepository;
        }

        public async Task<Result<CreateReservationResult>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            var result = await _deliveryReservationRepository.ReserveAsync(request.OrderId, request.UserId, request.DeliverySlotId, cancellationToken);

            switch (result.Status)
            {
                case ReserveDeliverySlotOperationStatus.Success:

                    return Result<CreateReservationResult>.Success(
                        new CreateReservationResult(result.ReservationId!.Value));

                case ReserveDeliverySlotOperationStatus.SlotNotAvailable:

                    return Result<CreateReservationResult>.Failure(DeliverySlotUnavailable);

                case ReserveDeliverySlotOperationStatus.InvalidReservationState:

                    return Result<CreateReservationResult>.Failure(InvalidReservationState);

                default:
                    return Result<CreateReservationResult>.Failure(UnknownError);
            }
        }
    }
}
