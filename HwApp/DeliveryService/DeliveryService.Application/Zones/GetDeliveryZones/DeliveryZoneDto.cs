namespace DeliveryService.Application.Zones.GetDeliveryZones
{
    public sealed record DeliveryZoneDto(
        Guid ZoneId,
        string Name,
        string City,
        bool IsActive);
}
