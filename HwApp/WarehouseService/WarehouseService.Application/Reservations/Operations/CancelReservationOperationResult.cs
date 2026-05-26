namespace WarehouseService.Application.Reservations.Operations
{
    public enum CancelReservationOperationResult
    {
        Success = 0,
        ReservationNotFound = 1,
        InvalidReservationState = 2,
        StockStateConflict = 3
    }
}
