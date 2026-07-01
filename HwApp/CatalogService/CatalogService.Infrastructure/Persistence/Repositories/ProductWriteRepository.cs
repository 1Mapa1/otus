using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Domain.Products;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductWriteRepository : IProductWriteRepository
    {
        private readonly CatalogWriteDbContext _context;

        public ProductWriteRepository(CatalogWriteDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            Product product,
            string brandName,
            string attributeValuesJson,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;

            await _context.Products.AddAsync(product, cancellationToken);

            var readModel = ProductReadModel.CreateForProduct(product, brandName, attributeValuesJson, utcNow);
            await _context.ProductReadModels.AddAsync(readModel, cancellationToken);
        }

        public Task<Product?> GetByIdWithAttributesAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return _context.Products
                .Include("_attributes")
                .FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
        }

        public async Task UpdateAsync(
            Product product,
            string brandName,
            string attributeValuesJson,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;

            var existingAttributes = await _context.ProductAttributes
                .Where(attribute => attribute.ProductId == product.Id)
                .ToListAsync(cancellationToken);

            _context.ProductAttributes.RemoveRange(existingAttributes);

            var readModel = await _context.ProductReadModels
                .FirstAsync(model => model.ProductId == product.Id, cancellationToken);

            readModel.SyncFromProduct(product, brandName, attributeValuesJson, utcNow);
        }

        public async Task ArchiveAsync(Product product, CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;

            var readModel = await _context.ProductReadModels
                .FirstAsync(model => model.ProductId == product.Id, cancellationToken);

            readModel.SyncFromProduct(
                product,
                readModel.BrandName,
                readModel.AttributeValuesJson,
                utcNow);
        }

        public async Task RestoreAsync(Product product, CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;

            var readModel = await _context.ProductReadModels
                .FirstAsync(model => model.ProductId == product.Id, cancellationToken);

            readModel.SyncFromProduct(
                product,
                readModel.BrandName,
                readModel.AttributeValuesJson,
                utcNow);
        }

        public async Task UpdateBrandNameInReadModelsAsync(
            Guid brandId,
            string brandName,
            CancellationToken cancellationToken = default)
        {
            await _context.ProductReadModels
                .Where(model => model.BrandId == brandId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(model => model.BrandName, brandName)
                        .SetProperty(model => model.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);
        }
    }
}
