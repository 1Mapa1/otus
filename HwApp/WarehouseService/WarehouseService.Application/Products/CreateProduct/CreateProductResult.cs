namespace WarehouseService.Application.Products.CreateProduct
{
    public sealed record CreateProductResult(
         Guid ProductId,
         string Name,
         decimal UnitPrice);
}
