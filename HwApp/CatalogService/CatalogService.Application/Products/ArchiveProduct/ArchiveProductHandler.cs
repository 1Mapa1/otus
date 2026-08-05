using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.ArchiveProduct
{
    internal sealed class ArchiveProductHandler : IRequestHandler<ArchiveProductCommand, Result>
    {
        private readonly IProductWriteRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ArchiveProductHandler(IProductWriteRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdWithAttributesAsync(request.ProductId, cancellationToken);
            if (product is null)
                return Result.Failure(new Error("ProductNotFound", "Product was not found.", ErrorType.NotFound));

            if (!product.IsActive)
                return Result.Success();

            product.Archive(DateTime.UtcNow);
            await _productRepository.ArchiveAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
