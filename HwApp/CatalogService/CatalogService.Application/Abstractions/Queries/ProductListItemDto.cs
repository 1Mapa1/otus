using CatalogService.Domain.Products;

namespace CatalogService.Application.Abstractions.Queries
{
    public sealed record ProductListItemDto(
        Guid ProductId,
        string Name,
        decimal Price,
        string ImageUrl,
        Guid BrandId,
        string BrandName,
        Guid CategoryId,
        IReadOnlyList<string> Attributes,
        AvailabilityStatus AvailabilityStatus);
}
