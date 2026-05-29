namespace OrderService.Application.Abstractions.Clients.Delivery
{
    public enum DeliveryClientErrorCode
    {
        SlotNotAvailable = 1,
        InvalidReservationState = 2,
        InvalidRequest = 3,
        Unknown = 100
    }
}
