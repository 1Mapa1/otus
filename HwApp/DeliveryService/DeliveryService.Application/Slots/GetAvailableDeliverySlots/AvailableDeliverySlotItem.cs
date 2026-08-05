namespace DeliveryService.Application.Slots.GetAvailableDeliverySlots
{
    public sealed record AvailableDeliverySlotItem(
        Guid SlotId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int AvailableCapacity);
}
