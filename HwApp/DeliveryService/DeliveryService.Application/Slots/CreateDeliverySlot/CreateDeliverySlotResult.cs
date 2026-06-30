namespace DeliveryService.Application.Slots.CreateDeliverySlot
{
    public sealed record CreateDeliverySlotResult(
        Guid SlotId,
        Guid ZoneId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int Capacity,
        int ReservedCount,
        string Status);
}
