namespace DeliveryService.Application.Slots.GetDeliverySlots
{
    public sealed record GetDeliverySlotsResult(
        IReadOnlyList<DeliverySlotDto> DeliverySlots);
}
