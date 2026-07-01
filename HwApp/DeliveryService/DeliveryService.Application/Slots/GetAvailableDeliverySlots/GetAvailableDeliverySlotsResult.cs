namespace DeliveryService.Application.Slots.GetAvailableDeliverySlots
{
    public sealed record GetAvailableDeliverySlotsResult(
        IReadOnlyList<AvailableDeliverySlotDto> DeliverySlots);
}
