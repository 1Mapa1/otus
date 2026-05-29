using OrderService.Application.Abstractions.Clients.Delivery;

namespace OrderService.Application.Abstractions.Clients.Delivery.CreateReservation
{
    public sealed record CreateReservationResult
    {
        public bool IsSuccess { get; init; }

        public Guid? ReservationId { get; init; }

        public DeliveryClientError? Error { get; init; }

        public static CreateReservationResult Success(Guid reservationId)
        {
            return new CreateReservationResult
            {
                IsSuccess = true,
                ReservationId = reservationId
            };
        }

        public static CreateReservationResult Failure(DeliveryClientError error)
        {
            return new CreateReservationResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
