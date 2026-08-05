using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.GetProductById
{
    internal sealed class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDetailsDto>>
    {
        private static readonly Error ProductNotFound = new(
            "ProductNotFound",
            "Product was not found.",
            ErrorType.NotFound);

        private readonly IProductReadRepository _productReadRepository;

        public GetProductByIdHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<Result<ProductDetailsDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await ReadResults.ExecuteAsync<ProductDetailsDto?>(
                ct => _productReadRepository.GetProductByIdAsync(request.ProductId, ct),
                cancellationToken);

            if (!result.IsSuccess)
                return Result<ProductDetailsDto>.Failure(result.Error!);

            if (result.Value is null)
                return Result<ProductDetailsDto>.Failure(ProductNotFound);

            return Result<ProductDetailsDto>.Success(result.Value);
        }
    }
}
