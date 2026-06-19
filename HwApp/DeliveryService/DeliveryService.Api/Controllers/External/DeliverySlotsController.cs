using DeliveryService.Api.Contracts;
using DeliveryService.Api.Extensions;
using DeliveryService.Application.Slots.CreateDeliverySlot;
using DeliveryService.Application.Slots.GetDeliverySlots;
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

        [HttpGet]
        public async Task<IActionResult> GetSlots(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetDeliverySlotsQuery(), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot(
            [FromBody] CreateDeliverySlotRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateDeliverySlotCommand(request.TimeFrom, request.TimeTo), cancellationToken);

            return result.ToActionResult();
        }
    }
}
