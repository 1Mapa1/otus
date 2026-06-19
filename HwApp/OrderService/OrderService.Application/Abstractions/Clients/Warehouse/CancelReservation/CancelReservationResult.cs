using OrderService.Application.Abstractions.Clients.Warehouse;

namespace OrderService.Application.Abstractions.Clients.Warehouse.CancelReservation
{
    public sealed record CancelReservationResult
    {
        public bool IsSuccess { get; init; }

        public WarehouseClientError? Error { get; init; }

        public static CancelReservationResult Success()
        {
            return new CancelReservationResult
            {
                IsSuccess = true
            };
        }

        public static CancelReservationResult Failure(WarehouseClientError error)
        {
            return new CancelReservationResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
