using CatalogService.Api.Contracts;
using CatalogService.Api.Extensions;
using CatalogService.Application.Brands.CreateBrand;
using CatalogService.Application.Brands.UpdateBrand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers.External
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/catalog/brands")]
    public sealed class AdminBrandsController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminBrandsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand(
            [FromBody] UpsertBrandRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateBrandCommand(request.Name), cancellationToken);

            if (result.IsSuccess)
                return Accepted(new { brandId = result.Value!.BrandId });

            return result.ToActionResult();
        }

        [HttpPut("{brandId:guid}")]
        public async Task<IActionResult> UpdateBrand(
            Guid brandId,
            [FromBody] UpsertBrandRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UpdateBrandCommand(brandId, request.Name), cancellationToken);
            return result.ToActionResult();
        }
    }
}
