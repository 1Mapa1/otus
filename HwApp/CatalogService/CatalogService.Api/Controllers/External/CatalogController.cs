using CatalogService.Api.Extensions;
using CatalogService.Application.Brands.GetBrands;
using CatalogService.Application.Categories.GetCategories;
using CatalogService.Application.Products.GetProductById;
using CatalogService.Application.Products.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers.External
{
    [ApiController]
    [Route("api/catalog")]
    public sealed class CatalogController : ControllerBase
    {
        private readonly ISender _sender;

        public CatalogController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? search,
            [FromQuery] Guid? brandId,
            [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sort,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetProductsQuery(search, brandId, categoryId, minPrice, maxPrice, sort, page, pageSize),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ToActionResult();

            var value = result.Value!;
            return Ok(new
            {
                items = value.Items.Select(item => new
                {
                    productId = item.ProductId,
                    name = item.Name,
                    price = item.Price,
                    imageUrl = item.ImageUrl,
                    brandId = item.BrandId,
                    brandName = item.BrandName,
                    categoryId = item.CategoryId,
                    attributes = item.Attributes,
                    availabilityStatus = item.AvailabilityStatus.ToString()
                }),
                page = value.Page,
                pageSize = value.PageSize,
                totalCount = value.TotalCount
            });
        }

        [HttpGet("products/{productId:guid}")]
        public async Task<IActionResult> GetProductById(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductByIdQuery(productId), cancellationToken);

            if (!result.IsSuccess)
                return result.ToActionResult();

            var product = result.Value!;
            return Ok(new
            {
                productId = product.ProductId,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                imageUrl = product.ImageUrl,
                brand = new { id = product.Brand.Id, name = product.Brand.Name },
                category = new { id = product.Category.Id, name = product.Category.Name },
                attributes = product.Attributes.Select(attribute => new
                {
                    name = attribute.Name,
                    value = attribute.Value,
                    sortOrder = attribute.SortOrder
                }),
                availabilityStatus = product.AvailabilityStatus.ToString()
            });
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetBrandsQuery(), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCategoriesQuery(), cancellationToken);
            return result.ToActionResult();
        }
    }
}
