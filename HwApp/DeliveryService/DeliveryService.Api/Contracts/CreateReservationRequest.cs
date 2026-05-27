namespace DeliveryService.Api.Contracts
{
    public sealed record CreateReservationRequest(
        Guid OrderId,
        Guid UserId,
        Guid DeliverySlotId);
}
