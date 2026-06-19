namespace WarehouseService.Application.Reservations.Operations
{
    public enum ReserveProductsOperationStatus
    {
        Success = 0,
        StockNotAvailable = 1,
        InvalidReservationState = 2,
        InvalidItems = 3
    }
}
