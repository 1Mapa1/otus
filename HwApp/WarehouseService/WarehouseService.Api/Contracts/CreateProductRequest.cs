namespace WarehouseService.Api.Contracts
{
    public sealed record CreateProductRequest(
        string Name,
        decimal UnitPrice);
}
