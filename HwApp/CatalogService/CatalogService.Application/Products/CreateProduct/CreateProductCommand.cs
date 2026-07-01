using CatalogService.Application.Common;
using CatalogService.Application.Products;
using MediatR;

namespace CatalogService.Application.Products.CreateProduct
{
    public sealed record CreateProductCommand(
        string Name,
        string Description,
        Guid BrandId,
        Guid CategoryId,
        decimal Price,
        string ImageUrl,
        IReadOnlyList<ProductAttributeInput> Attributes) : IRequest<Result<CreateProductResult>>;
}
