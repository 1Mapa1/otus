namespace OrderService.Application.Abstractions.Clients.Warehouse
{
    public enum WarehouseClientErrorCode
    {
        StockNotAvailable = 1,
        ProductNotFound = 2,
        InvalidItems = 3,
        InvalidReservationState = 4,
        Unknown = 100
    }
}
