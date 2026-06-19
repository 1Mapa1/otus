using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarehouseService.Api.Contracts;
using WarehouseService.Api.Extensions;
using WarehouseService.Application.Products.ResolveProducts;

namespace WarehouseService.Api.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/warehouse/products")]
    public sealed class InternalWarehouseProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public InternalWarehouseProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("resolve")]
        public async Task<IActionResult> Resolve(
            [FromBody] ResolveProductsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new ResolveProductsCommand(request.Items), cancellationToken);

            return result.ToActionResult();
        }
    }
}
