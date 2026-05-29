namespace DeliveryService.Api.Contracts
{
    public sealed record CreateDeliverySlotRequest(
        DateTime TimeFrom,
        DateTime TimeTo);
}
