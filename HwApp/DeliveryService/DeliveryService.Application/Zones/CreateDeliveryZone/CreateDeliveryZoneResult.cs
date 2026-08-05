namespace DeliveryService.Application.Zones.CreateDeliveryZone
{
    public sealed record CreateDeliveryZoneResult(
        Guid ZoneId,
        string Name,
        string City,
        bool IsActive);
}
