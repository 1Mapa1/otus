namespace DeliveryService.Application.Slots.GetAvailableDeliverySlots
{
    public sealed record AvailableDeliverySlotDto(
        Guid SlotId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int AvailableCapacity);
}
