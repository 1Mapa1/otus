namespace OrderService.Application.Abstractions.Clients.Warehouse
{
    public sealed record WarehouseClientError(
        WarehouseClientErrorCode Code,
        string? Message = null);
}
