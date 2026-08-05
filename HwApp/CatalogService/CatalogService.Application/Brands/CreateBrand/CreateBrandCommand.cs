using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Brands.CreateBrand
{
    public sealed record CreateBrandCommand(string Name) : IRequest<Result<CreateBrandResult>>;
}
