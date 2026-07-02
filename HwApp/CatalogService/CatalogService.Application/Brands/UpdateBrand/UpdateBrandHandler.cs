using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Brands.UpdateBrand
{
    internal sealed class UpdateBrandHandler : IRequestHandler<UpdateBrandCommand, Result>
    {
        private readonly IBrandWriteRepository _brandRepository;
        private readonly IProductWriteRepository _productRepository;
        private readonly IBrandPrimaryReadRepository _brandPrimaryReadRepository;
        private readonly ICatalogReferenceListCacheService _referenceListCacheService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBrandHandler(
            IBrandWriteRepository brandRepository,
            IProductWriteRepository productRepository,
            IBrandPrimaryReadRepository brandPrimaryReadRepository,
            ICatalogReferenceListCacheService referenceListCacheService,
            IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _brandPrimaryReadRepository = brandPrimaryReadRepository;
            _referenceListCacheService = referenceListCacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Failure(new Error("ValidationError", "Name is required.", ErrorType.Validation));

            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
            if (brand is null)
                return Result.Failure(new Error("BrandNotFound", "Brand was not found.", ErrorType.NotFound));

            if (await _brandRepository.ExistsByNameAsync(request.Name, request.BrandId, cancellationToken))
                return Result.Failure(new Error("BrandNameConflict", "Brand name already exists.", ErrorType.Conflict));

            brand.Rename(request.Name, DateTime.UtcNow);
            await _productRepository.UpdateBrandNameInReadModelsAsync(brand.Id, brand.Name, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _referenceListCacheService.RefreshBrandsAsync(
                ct => _brandPrimaryReadRepository.GetBrandsAsync(ct),
                cancellationToken);

            return Result.Success();
        }
    }
}
