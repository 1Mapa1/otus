namespace DeliveryService.Application.Slots.UpdateDeliverySlot
{
    public sealed record UpdateDeliverySlotResult(
        Guid SlotId,
        Guid ZoneId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int Capacity,
        int ReservedCount,
        string Status);
}
