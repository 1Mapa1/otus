using DeliveryService.Api.Contracts;
using DeliveryService.Api.Extensions;
using DeliveryService.Application.Slots.GetAvailableDeliverySlots;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Api.Controllers.External
{
    [ApiController]
    [Authorize]
    [Route("api/delivery/slots")]
    public sealed class DeliverySlotsController : ControllerBase
    {
        private readonly ISender _sender;

        public DeliverySlotsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("available")]
        public async Task<IActionResult> GetAvailableSlots(
            [FromBody] GetAvailableDeliverySlotsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAvailableDeliverySlotsQuery(
                    request.Address.City,
                    request.Address.Street,
                    request.Address.House,
                    request.Address.Apartment),
                cancellationToken);

            return result.ToActionResult();
        }
    }
}
