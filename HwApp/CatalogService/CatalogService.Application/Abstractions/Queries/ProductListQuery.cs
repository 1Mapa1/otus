namespace CatalogService.Application.Abstractions.Queries
{
    public sealed record ProductListQuery(
        string? Search,
        Guid? BrandId,
        Guid? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        string Sort,
        int Page,
        int PageSize);
}
