namespace DeliveryService.Application.Zones.UpdateDeliveryZone
{
    public sealed record UpdateDeliveryZoneResult(
        Guid ZoneId,
        string Name,
        string City,
        bool IsActive);
}
