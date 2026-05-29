using MediatR;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Products.ResolveProducts
{
    internal sealed class ResolveProductsHandler : IRequestHandler<ResolveProductsCommand, Result<ResolveProductsResult>>
    {
        private static readonly Error ValidateItems = new("ValidateItems", "The reservation must contain at least one item.", ErrorType.Validation);
        private static readonly Error ValidateItemPositiveQuantity = new("ValidateItemPositiveQuantity", "The reservation only positive quantity.", ErrorType.Validation);
        private static Error ProductNotFound(Guid productId) => new("ProductNotFound", $"Product '{productId}' was not found.", ErrorType.NotFound);
        private static Error StockNotAvailable(Guid productId) => new("StockNotAvailable", $"Insufficient stock for product '{productId}'.", ErrorType.Conflict);

        private readonly IProductRepository _productRepository;

        public ResolveProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<ResolveProductsResult>> Handle(ResolveProductsCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || !request.Items.Any())
                return Result<ResolveProductsResult>.Failure(ValidateItems);

            if (request.Items.Any(x => x.Quantity <= 0))
                return Result<ResolveProductsResult>.Failure(ValidateItemPositiveQuantity);

            var products = await _productRepository
                .GetProductsByIdsAsync(
                    request.Items.Select(i => i.ProductId).Distinct(), 
                    cancellationToken);

            var productsById = products.ToDictionary(x => x.Id);

            var resolvedItems = new List<ResolvedProductItemDto>();

            foreach (var item in request.Items)
            {
                if (!productsById.TryGetValue(item.ProductId, out var product))
                    return Result<ResolveProductsResult>.Failure(ProductNotFound(item.ProductId));

                if (item.Quantity > product.FreeQuantity)
                    return Result<ResolveProductsResult>.Failure(StockNotAvailable(product.Id));

                resolvedItems.Add(new ResolvedProductItemDto
                (
                    product.Id,
                    product.Name,
                    product.UnitPrice,
                    product.FreeQuantity,
                    product.UnitPrice * item.Quantity
                ));
            }

            return Result<ResolveProductsResult>.Success(
                new ResolveProductsResult(
                    resolvedItems,
                    resolvedItems.Sum(x => x.TotalPrice)));
        }
    }
}
