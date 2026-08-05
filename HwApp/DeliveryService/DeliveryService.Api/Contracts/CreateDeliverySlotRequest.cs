namespace DeliveryService.Api.Contracts
{
    public sealed record CreateDeliverySlotRequest(
        Guid ZoneId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int Capacity);
}
