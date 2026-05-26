using MediatR;
using WarehouseService.Application.Common;
using WarehouseService.Application.Reservations.Operations;

namespace WarehouseService.Application.Reservations.CreateReservation
{
    public sealed class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Result<CreateReservationResult>>
    {
        private static readonly Error ValidateItems = new("ValidateItems", "The reservation must contain at least one item.", ErrorType.Validation);
        private static readonly Error ValidateItemQuantity = new("ValidateItemQuantity", "The reservation only positive quantity.", ErrorType.Validation);
        private static readonly Error InvalidReservationState = new("InvalidReservationState", "The reservation is in an invalid state for this operation.", ErrorType.Validation);
        private static readonly Error UnknownError = new("UnknownError", "An unknown error occurred while creating the reservation.", ErrorType.Failure);
        private static Error StockNotAvailable(IEnumerable<UnavailableStockItem>? unavailableItems) => 
            new("StockNotAvailable", "The following items are not available in the requested quantity", ErrorType.Conflict, new StockNotAvailableError("StockNotAvailable", unavailableItems?.ToList() ?? []));

        private readonly IReservationRepository _reservationRepository;

        public CreateReservationHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<Result<CreateReservationResult>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || !request.Items.Any())
                return Result<CreateReservationResult>.Failure(ValidateItems);

            if (request.Items.Any(x => x.Quantity <= 0))
                return Result<CreateReservationResult>.Failure(ValidateItemQuantity);

            var reserveProductItems = request.Items.Select(x => new ReserveProductItem(x.ProductId, x.Quantity)).ToList();

            var result = await _reservationRepository.ReserveAsync(request.OrderId, request.UserId, reserveProductItems, cancellationToken);

            switch (result.Status)
            {
                case ReserveProductsOperationStatus.Success:

                    return Result<CreateReservationResult>.Success(
                        new CreateReservationResult(result.ReservationId!.Value ));

                case ReserveProductsOperationStatus.InvalidItems:

                    return Result<CreateReservationResult>.Failure(ValidateItems);

                case ReserveProductsOperationStatus.InvalidReservationState:

                    return Result<CreateReservationResult>.Failure(InvalidReservationState);

                case ReserveProductsOperationStatus.StockNotAvailable:

                    return Result<CreateReservationResult>.Failure(StockNotAvailable(result.UnavailableItems));

                default:
                    return Result<CreateReservationResult>.Failure(UnknownError);
            }
        }
    }
}
