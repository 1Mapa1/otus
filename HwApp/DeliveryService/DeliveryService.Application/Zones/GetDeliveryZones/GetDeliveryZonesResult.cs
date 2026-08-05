namespace DeliveryService.Application.Zones.GetDeliveryZones
{
    public sealed record GetDeliveryZonesResult(
        IReadOnlyList<DeliveryZoneDto> Zones);
}
