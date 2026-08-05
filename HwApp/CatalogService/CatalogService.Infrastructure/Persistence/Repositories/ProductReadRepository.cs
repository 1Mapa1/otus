using System.Text.Json;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductReadRepository : IProductReadRepository
    {
        private readonly CatalogReadDbContext _context;

        public ProductReadRepository(CatalogReadDbContext context)
        {
            _context = context;
        }

        public async Task<ProductListResultDto> GetProductsAsync(
            ProductListQuery query,
            CancellationToken cancellationToken = default)
        {
            var productsQuery = _context.ProductReadModels
                .AsNoTracking()
                .Where(model => model.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var pattern = $"%{query.Search.Trim()}%";
                productsQuery = productsQuery.Where(model =>
                    EF.Functions.ILike(model.Name, pattern) ||
                    EF.Functions.ILike(model.BrandName, pattern));
            }

            if (query.BrandId is not null)
                productsQuery = productsQuery.Where(model => model.BrandId == query.BrandId);

            if (query.CategoryId is not null)
                productsQuery = productsQuery.Where(model => model.CategoryId == query.CategoryId);

            if (query.MinPrice is not null)
                productsQuery = productsQuery.Where(model => model.Price >= query.MinPrice);

            if (query.MaxPrice is not null)
                productsQuery = productsQuery.Where(model => model.Price <= query.MaxPrice);

            productsQuery = query.Sort switch
            {
                "price-asc" => productsQuery.OrderBy(model => model.Price).ThenBy(model => model.Name),
                "price-desc" => productsQuery.OrderByDescending(model => model.Price).ThenBy(model => model.Name),
                "name-desc" => productsQuery.OrderByDescending(model => model.Name),
                _ => productsQuery.OrderBy(model => model.Name)
            };

            var totalCount = await productsQuery.CountAsync(cancellationToken);

            var items = await productsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var mappedItems = items
                .Select(model => new ProductListItemDto(
                    model.ProductId,
                    model.Name,
                    model.Price,
                    model.ImageUrl,
                    model.BrandId,
                    model.BrandName,
                    model.CategoryId,
                    ParseAttributeValues(model.AttributeValuesJson),
                    model.AvailabilityStatus))
                .ToList();

            return new ProductListResultDto(mappedItems, query.Page, query.PageSize, totalCount);
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == productId && item.IsActive, cancellationToken);

            if (product is null)
                return null;

            var brand = await _context.Brands
                .AsNoTracking()
                .FirstAsync(item => item.Id == product.BrandId, cancellationToken);

            var category = await _context.Categories
                .AsNoTracking()
                .FirstAsync(item => item.Id == product.CategoryId, cancellationToken);

            var attributes = await _context.ProductAttributes
                .AsNoTracking()
                .Where(attribute => attribute.ProductId == productId)
                .OrderBy(attribute => attribute.SortOrder)
                .Select(attribute => new ProductAttributeDto(attribute.Name, attribute.Value, attribute.SortOrder))
                .ToListAsync(cancellationToken);

            var availabilityStatus = await _context.ProductReadModels
                .AsNoTracking()
                .Where(model => model.ProductId == productId)
                .Select(model => (AvailabilityStatus?)model.AvailabilityStatus)
                .FirstOrDefaultAsync(cancellationToken)
                ?? AvailabilityStatus.OutOfStock;

            return new ProductDetailsDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl,
                new BrandRefDto(brand.Id, brand.Name),
                new CategoryRefDto(category.Id, category.Name),
                attributes,
                availabilityStatus);
        }

        private static IReadOnlyList<string> ParseAttributeValues(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return [];

            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
    }
}
