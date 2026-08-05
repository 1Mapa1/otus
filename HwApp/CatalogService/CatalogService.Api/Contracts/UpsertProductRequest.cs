namespace CatalogService.Api.Contracts
{
    public sealed record UpsertProductRequest(
        string Name,
        string Description,
        Guid BrandId,
        Guid CategoryId,
        decimal Price,
        string ImageUrl,
        IReadOnlyList<ProductAttributeRequest> Attributes);
}
