namespace OrderService.Infrastructure.Clients.Warehouse.Dto
{
    public sealed record ProductQuantityDto(
        Guid ProductId,
        int Quantity);
}
