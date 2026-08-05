using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Categories.GetCategories
{
    public sealed record GetCategoriesQuery() : IRequest<Result<IReadOnlyList<CategoryListItemDto>>>;
}
