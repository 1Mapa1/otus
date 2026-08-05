using DeliveryService.Api.Contracts;
using DeliveryService.Api.Extensions;
using DeliveryService.Application.Zones.CreateDeliveryZone;
using DeliveryService.Application.Zones.GetDeliveryZones;
using DeliveryService.Application.Zones.UpdateDeliveryZone;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Api.Controllers.External
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/delivery/zones")]
    public sealed class AdminDeliveryZonesController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminDeliveryZonesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetZones(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetDeliveryZonesQuery(), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateZone(
            [FromBody] CreateDeliveryZoneRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateDeliveryZoneCommand(request.Name, request.City),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{zoneId:guid}")]
        public async Task<IActionResult> UpdateZone(
            Guid zoneId,
            [FromBody] UpdateDeliveryZoneRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateDeliveryZoneCommand(zoneId, request.Name, request.IsActive),
                cancellationToken);

            return result.ToActionResult();
        }
    }
}
