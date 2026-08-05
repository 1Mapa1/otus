namespace OrderService.Application.Abstractions.Clients.Warehouse
{
    public enum WarehouseClientErrorCode
    {
        StockNotAvailable = 1,
        InvalidReservationState = 2,
        Unknown = 100
    }
}
