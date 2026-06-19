namespace DeliveryService.Application.Reservations.Operations
{
    public sealed record ReserveDeliverySlotOperationResult(
        ReserveDeliverySlotOperationStatus Status,
        Guid? ReservationId = null)
    {
        public static ReserveDeliverySlotOperationResult Success(Guid reservationId)
           => new(
               ReserveDeliverySlotOperationStatus.Success,
               ReservationId: reservationId);

        public static ReserveDeliverySlotOperationResult SlotNotAvailable()
            => new(ReserveDeliverySlotOperationStatus.SlotNotAvailable);

        public static ReserveDeliverySlotOperationResult InvalidReservationState()
            => new(ReserveDeliverySlotOperationStatus.InvalidReservationState);
    }
}
