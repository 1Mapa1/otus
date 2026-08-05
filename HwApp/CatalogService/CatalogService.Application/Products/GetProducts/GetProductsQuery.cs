using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.GetProducts
{
    public sealed record GetProductsQuery(
        string? Search,
        Guid? BrandId,
        Guid? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        string? Sort,
        int? Page,
        int? PageSize) : IRequest<Result<ProductListResultDto>>;
}
