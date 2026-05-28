namespace OrderService.Application.Abstractions.Warehouse
{
    public sealed record WarehouseClientError(
        WarehouseClientErrorCode Code,
        string? Message = null);
}
