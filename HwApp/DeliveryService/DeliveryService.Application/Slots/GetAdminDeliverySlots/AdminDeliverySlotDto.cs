namespace DeliveryService.Application.Slots.GetAdminDeliverySlots
{
    public sealed record AdminDeliverySlotDto(
        Guid SlotId,
        Guid ZoneId,
        string ZoneName,
        string ZoneCity,
        DateTime TimeFrom,
        DateTime TimeTo,
        int Capacity,
        int ReservedCount,
        string Status);
}
