using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.GetProducts
{
    internal sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<ProductListResultDto>>
    {
        private const int DefaultPage = 1;
        private const int DefaultPageSize = 24;
        private const int MaxPageSize = 100;
        private const string DefaultSort = "name-asc";

        private static readonly HashSet<string> AllowedSorts = new(StringComparer.Ordinal)
        {
            "price-asc",
            "price-desc",
            "name-asc",
            "name-desc"
        };

        private readonly IProductListCacheService _cacheService;
        private readonly IProductReadRepository _productReadRepository;

        public GetProductsHandler(
            IProductListCacheService cacheService,
            IProductReadRepository productReadRepository)
        {
            _cacheService = cacheService;
            _productReadRepository = productReadRepository;
        }

        public async Task<Result<ProductListResultDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var page = request.Page.GetValueOrDefault(DefaultPage);
            var pageSize = request.PageSize.GetValueOrDefault(DefaultPageSize);
            var sort = string.IsNullOrWhiteSpace(request.Sort) ? DefaultSort : request.Sort.Trim();

            if (page < 1)
                return Result<ProductListResultDto>.Failure(new Error("ValidationError", "Page must be greater than zero.", ErrorType.Validation));

            if (pageSize < 1 || pageSize > MaxPageSize)
                return Result<ProductListResultDto>.Failure(new Error("ValidationError", $"PageSize must be between 1 and {MaxPageSize}.", ErrorType.Validation));

            if (!AllowedSorts.Contains(sort))
                return Result<ProductListResultDto>.Failure(new Error("ValidationError", "Sort value is not supported.", ErrorType.Validation));

            if (request.MinPrice is not null && request.MaxPrice is not null && request.MinPrice > request.MaxPrice)
                return Result<ProductListResultDto>.Failure(new Error("ValidationError", "MinPrice cannot be greater than MaxPrice.", ErrorType.Validation));

            var query = new ProductListQuery(
                request.Search,
                request.BrandId,
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                sort,
                page,
                pageSize);

            return await ReadResults.ExecuteAsync(
                cancellationToken => _cacheService.GetOrLoadAsync(
                    query,
                    ct => _productReadRepository.GetProductsAsync(query, ct),
                    cancellationToken),
                cancellationToken);
        }
    }
}
