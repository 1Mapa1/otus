namespace DeliveryService.Api.Contracts
{
    public sealed record UpdateDeliverySlotRequest(
        Guid ZoneId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int Capacity,
        string Status);
}
