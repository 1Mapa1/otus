using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Brands.UpdateBrand
{
    public sealed record UpdateBrandCommand(Guid BrandId, string Name) : IRequest<Result>;
}
