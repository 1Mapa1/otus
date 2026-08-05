using CatalogService.Domain.Products;

namespace CatalogService.Application.Abstractions.Queries
{
    public sealed record ProductDetailsDto(
        Guid ProductId,
        string Name,
        string Description,
        decimal Price,
        string ImageUrl,
        BrandRefDto Brand,
        CategoryRefDto Category,
        IReadOnlyList<ProductAttributeDto> Attributes,
        AvailabilityStatus AvailabilityStatus);
}
