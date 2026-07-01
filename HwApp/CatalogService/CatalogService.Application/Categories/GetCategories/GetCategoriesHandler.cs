using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Categories.GetCategories
{
    internal sealed class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryListItemDto>>>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;

        public GetCategoriesHandler(ICategoryReadRepository categoryReadRepository)
        {
            _categoryReadRepository = categoryReadRepository;
        }

        public async Task<Result<IReadOnlyList<CategoryListItemDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await ReadResults.ExecuteAsync(
                cancellationToken => _categoryReadRepository.GetCategoriesAsync(cancellationToken),
                cancellationToken);
        }
    }
}
