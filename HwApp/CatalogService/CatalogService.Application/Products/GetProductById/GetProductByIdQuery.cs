using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.GetProductById
{
    public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductDetailsDto>>;
}
