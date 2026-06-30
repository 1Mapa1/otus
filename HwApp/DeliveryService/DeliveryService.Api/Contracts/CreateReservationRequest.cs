namespace DeliveryService.Api.Contracts
{
    public sealed record CreateReservationRequest(
        Guid OrderId,
        Guid CustomerId,
        Guid DeliverySlotId,
        DeliveryAddressRequest Address);
}
