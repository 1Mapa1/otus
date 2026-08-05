using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Brands.GetBrands
{
    internal sealed class GetBrandsHandler : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandListItemDto>>>
    {
        private readonly IBrandReadRepository _brandReadRepository;
        private readonly ICatalogReferenceListCacheService _referenceListCacheService;

        public GetBrandsHandler(
            IBrandReadRepository brandReadRepository,
            ICatalogReferenceListCacheService referenceListCacheService)
        {
            _brandReadRepository = brandReadRepository;
            _referenceListCacheService = referenceListCacheService;
        }

        public async Task<Result<IReadOnlyList<BrandListItemDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            return await ReadResults.ExecuteAsync(
                ct => _referenceListCacheService.GetBrandsAsync(
                    innerCt => _brandReadRepository.GetBrandsAsync(innerCt),
                    ct),
                cancellationToken);
        }
    }
}
