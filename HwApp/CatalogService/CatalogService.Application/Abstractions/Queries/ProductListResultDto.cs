namespace CatalogService.Application.Abstractions.Queries
{
    public sealed record ProductListResultDto(
        IReadOnlyList<ProductListItemDto> Items,
        int Page,
        int PageSize,
        int TotalCount);
}
