namespace DeliveryService.Api.Contracts
{
    public sealed record CancelReservationRequest(
        Guid OrderId);
}
