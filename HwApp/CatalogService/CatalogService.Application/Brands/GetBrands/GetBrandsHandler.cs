using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Brands.GetBrands
{
    internal sealed class GetBrandsHandler : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandListItemDto>>>
    {
        private readonly IBrandReadRepository _brandReadRepository;

        public GetBrandsHandler(IBrandReadRepository brandReadRepository)
        {
            _brandReadRepository = brandReadRepository;
        }

        public async Task<Result<IReadOnlyList<BrandListItemDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            return await ReadResults.ExecuteAsync(
                cancellationToken => _brandReadRepository.GetBrandsAsync(cancellationToken),
                cancellationToken);
        }
    }
}
