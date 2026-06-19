using MediatR;
using WarehouseService.Application.Abstractions;
using WarehouseService.Application.Common;
using WarehouseService.Domain.Products;

namespace WarehouseService.Application.Products.CreateProduct
{
    internal sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResult>>
    {
        private static readonly Error ValidateUnitPrice = new("ValidateUnitPrice", "Unit price cannot be negative.", ErrorType.Validation);

        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateProductResult>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            if (command.UnitPrice < 0)
                return Result<CreateProductResult>.Failure(ValidateUnitPrice);

            var product = Product.Create(command.Name, command.UnitPrice);

            await _productRepository.AddAsync(product, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreateProductResult>.Success(
                new CreateProductResult(
                    product.Id,
                    product.Name,
                    product.UnitPrice));
        }
    }
}
