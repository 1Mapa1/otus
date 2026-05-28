namespace OrderService.Infrastructure.Clients.Warehouse.Dto
{
    internal sealed record ProductUnavailableDto(
        Guid ProductId,
        int RequestedQuantity,
        int FreeQuantity);
}
