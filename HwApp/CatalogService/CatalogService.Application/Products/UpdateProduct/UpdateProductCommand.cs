using CatalogService.Application.Common;
using CatalogService.Application.Products;
using MediatR;

namespace CatalogService.Application.Products.UpdateProduct
{
    public sealed record UpdateProductCommand(
        Guid ProductId,
        string Name,
        string Description,
        Guid BrandId,
        Guid CategoryId,
        decimal Price,
        string ImageUrl,
        IReadOnlyList<ProductAttributeInput> Attributes) : IRequest<Result>;
}
