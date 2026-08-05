using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Application.Abstractions.Caching
{
    public interface IProductListCacheService
    {
        Task<ProductListResultDto> GetOrLoadAsync(
            ProductListQuery query,
            Func<CancellationToken, Task<ProductListResultDto>> loader,
            CancellationToken cancellationToken = default);
    }
}
