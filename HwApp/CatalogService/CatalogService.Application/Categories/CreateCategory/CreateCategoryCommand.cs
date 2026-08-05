using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Categories.CreateCategory
{
    public sealed record CreateCategoryCommand(string Name) : IRequest<Result<CreateCategoryResult>>;
}
