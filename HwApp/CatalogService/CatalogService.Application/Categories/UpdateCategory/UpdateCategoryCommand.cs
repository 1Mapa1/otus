using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Categories.UpdateCategory
{
    public sealed record UpdateCategoryCommand(Guid CategoryId, string Name) : IRequest<Result>;
}
