namespace OrderService.Application.Abstractions.Warehouse.CreateReservation
{
    public sealed record CreateReservationResult
    {
        public bool IsSuccess { get; init; }

        public Guid? ReservationId { get; init; }

        public WarehouseClientError? Error { get; init; }

        public IReadOnlyList<UnavailableProductItem> UnavailableItems { get; init; } = [];

        public static CreateReservationResult Success(Guid reservationId)
        {
            return new CreateReservationResult
            {
                IsSuccess = true,
                ReservationId = reservationId
            };
        }

        public static CreateReservationResult Failure(
            WarehouseClientError error,
            IReadOnlyList<UnavailableProductItem>? unavailableItems = null)
        {
            return new CreateReservationResult
            {
                IsSuccess = false,
                Error = error,
                UnavailableItems = unavailableItems ?? []
            };
        }
    }
}
