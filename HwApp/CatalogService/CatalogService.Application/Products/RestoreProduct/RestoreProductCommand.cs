using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.RestoreProduct
{
    public sealed record RestoreProductCommand(Guid ProductId) : IRequest<Result>;
}
