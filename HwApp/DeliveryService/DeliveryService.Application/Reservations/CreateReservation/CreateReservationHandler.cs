using DeliveryService.Application.Common;
using DeliveryService.Application.Reservations;
using DeliveryService.Application.Reservations.Operations;
using DeliveryService.Domain.Reservations;
using MediatR;

namespace DeliveryService.Application.Reservations.CreateReservation
{
    internal sealed class CreateReservationHandler
        : IRequestHandler<CreateReservationCommand, Result<CreateReservationResult>>
    {
        private static readonly Error ValidateCity = new("ValidateCity", "City is required.", ErrorType.Validation);
        private static readonly Error ValidateStreet = new("ValidateStreet", "Street is required.", ErrorType.Validation);
        private static readonly Error ValidateHouse = new("ValidateHouse", "House is required.", ErrorType.Validation);
        private static readonly Error DeliverySlotUnavailable = new("DeliverySlotUnavailable", "The delivery slot is unavailable.", ErrorType.Conflict);
        private static readonly Error InvalidReservationState = new("InvalidReservationState", "The reservation is in an invalid state for this operation.", ErrorType.Conflict);
        private static readonly Error UnknownError = new("UnknownError", "An unknown error occurred while creating the reservation.", ErrorType.Failure);

        private readonly IDeliveryReservationRepository _deliveryReservationRepository;

        public CreateReservationHandler(IDeliveryReservationRepository deliveryReservationRepository)
        {
            _deliveryReservationRepository = deliveryReservationRepository;
        }

        public async Task<Result<CreateReservationResult>> Handle(
            CreateReservationCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.City))
                return Result<CreateReservationResult>.Failure(ValidateCity);

            if (string.IsNullOrWhiteSpace(request.Street))
                return Result<CreateReservationResult>.Failure(ValidateStreet);

            if (string.IsNullOrWhiteSpace(request.House))
                return Result<CreateReservationResult>.Failure(ValidateHouse);

            var address = DeliveryAddress.Create(
                request.City,
                request.Street,
                request.House,
                request.Apartment);

            var result = await _deliveryReservationRepository.ReserveAsync(
                request.OrderId,
                request.CustomerId,
                request.DeliverySlotId,
                address,
                cancellationToken);

            return result.Status switch
            {
                ReserveDeliverySlotOperationStatus.Success =>
                    Result<CreateReservationResult>.Success(
                        new CreateReservationResult(result.ReservationId!.Value)),

                ReserveDeliverySlotOperationStatus.SlotNotAvailable =>
                    Result<CreateReservationResult>.Failure(DeliverySlotUnavailable),

                ReserveDeliverySlotOperationStatus.InvalidReservationState =>
                    Result<CreateReservationResult>.Failure(InvalidReservationState),

                _ => Result<CreateReservationResult>.Failure(UnknownError)
            };
        }
    }
}
