using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using CatalogService.Domain.Brands;
using MediatR;

namespace CatalogService.Application.Brands.CreateBrand
{
    internal sealed class CreateBrandHandler : IRequestHandler<CreateBrandCommand, Result<CreateBrandResult>>
    {
        private readonly IBrandWriteRepository _brandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBrandHandler(IBrandWriteRepository brandRepository, IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateBrandResult>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<CreateBrandResult>.Failure(new Error("ValidationError", "Name is required.", ErrorType.Validation));

            if (await _brandRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
                return Result<CreateBrandResult>.Failure(new Error("BrandNameConflict", "Brand name already exists.", ErrorType.Conflict));

            var brand = Brand.Create(request.Name, DateTime.UtcNow);
            await _brandRepository.AddAsync(brand, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreateBrandResult>.Success(new CreateBrandResult(brand.Id));
        }
    }
}
