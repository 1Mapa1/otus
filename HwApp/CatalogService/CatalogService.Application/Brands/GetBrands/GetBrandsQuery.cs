using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Brands.GetBrands
{
    public sealed record GetBrandsQuery() : IRequest<Result<IReadOnlyList<BrandListItemDto>>>;
}
