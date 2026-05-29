using OrderService.Application.Abstractions.Clients.Delivery;

namespace OrderService.Application.Abstractions.Clients.Delivery.CancelReservation
{
    public sealed record CancelReservationResult
    {
        public bool IsSuccess { get; init; }

        public DeliveryClientError? Error { get; init; }

        public static CancelReservationResult Success()
        {
            return new CancelReservationResult
            {
                IsSuccess = true
            };
        }

        public static CancelReservationResult Failure(DeliveryClientError error)
        {
            return new CancelReservationResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
