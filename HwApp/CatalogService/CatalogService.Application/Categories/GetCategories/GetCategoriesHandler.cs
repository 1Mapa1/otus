using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Categories.GetCategories
{
    internal sealed class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryListItemDto>>>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly ICatalogReferenceListCacheService _referenceListCacheService;

        public GetCategoriesHandler(
            ICategoryReadRepository categoryReadRepository,
            ICatalogReferenceListCacheService referenceListCacheService)
        {
            _categoryReadRepository = categoryReadRepository;
            _referenceListCacheService = referenceListCacheService;
        }

        public async Task<Result<IReadOnlyList<CategoryListItemDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await ReadResults.ExecuteAsync(
                ct => _referenceListCacheService.GetCategoriesAsync(
                    innerCt => _categoryReadRepository.GetCategoriesAsync(innerCt),
                    ct),
                cancellationToken);
        }
    }
}
