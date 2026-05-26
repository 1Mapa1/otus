using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Products.GetProducts
{
    public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<GetProductsResult>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<GetProductsResult>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);

            return Result<GetProductsResult>.Success(
                new GetProductsResult(
                    products.Select(x => new ProductDetailsDto(
                        x.Id,
                        x.Name,
                        x.UnitPrice,
                        x.AvailableQuantity,
                        x.ReservedQuantity,
                        x.FreeQuantity
                    )).ToList()
                )
            );
        }
    }
}
