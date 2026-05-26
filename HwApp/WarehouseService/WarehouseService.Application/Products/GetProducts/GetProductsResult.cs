namespace WarehouseService.Application.Products.GetProducts
{
    public sealed record GetProductsResult(
        IReadOnlyList<ProductDetailsDto> Items);
}