namespace DeliveryService.Application.Reservations.Operations
{
    public enum ReserveDeliverySlotOperationStatus
    {
        Success = 0,
        SlotNotAvailable = 1,
        InvalidReservationState = 2
    }
}