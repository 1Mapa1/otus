namespace OrderService.Infrastructure.Clients.Warehouse.Dto
{
    internal sealed record ProductDetailsDto(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
