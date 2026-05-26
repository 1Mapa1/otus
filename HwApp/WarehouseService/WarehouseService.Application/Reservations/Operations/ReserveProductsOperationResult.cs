namespace WarehouseService.Application.Reservations.Operations
{
    public sealed record ReserveProductsOperationResult(
        ReserveProductsOperationStatus Status,
        Guid? ReservationId = null,
        IReadOnlyCollection<UnavailableStockItem>? UnavailableItems = null)
    {
        public static ReserveProductsOperationResult Success(Guid reservationId)
            => new(
                ReserveProductsOperationStatus.Success,
                ReservationId: reservationId);

        public static ReserveProductsOperationResult StockNotAvailable(
            IReadOnlyCollection<UnavailableStockItem> unavailableItems)
            => new(
                ReserveProductsOperationStatus.StockNotAvailable,
                UnavailableItems: unavailableItems);

        public static ReserveProductsOperationResult InvalidReservationState()
            => new(ReserveProductsOperationStatus.InvalidReservationState);

        public static ReserveProductsOperationResult InvalidItems()
            => new(ReserveProductsOperationStatus.InvalidItems);
    }
}
