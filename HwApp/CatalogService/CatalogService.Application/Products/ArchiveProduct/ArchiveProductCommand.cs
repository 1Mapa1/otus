using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.ArchiveProduct
{
    public sealed record ArchiveProductCommand(Guid ProductId) : IRequest<Result>;
}
