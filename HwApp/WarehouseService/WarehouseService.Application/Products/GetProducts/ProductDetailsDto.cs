namespace WarehouseService.Application.Products.GetProducts
{
    public sealed record ProductDetailsDto(
        Guid ProductId,
        string Name,
        decimal UnitPrice,
        int AvailableQuantity,
        int ReservedQuantity,
        int FreeQuantity);
}
