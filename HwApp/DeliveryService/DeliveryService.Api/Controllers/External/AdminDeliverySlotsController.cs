using DeliveryService.Api.Contracts;
using DeliveryService.Api.Extensions;
using DeliveryService.Application.Slots.CreateDeliverySlot;
using DeliveryService.Application.Slots.GetAdminDeliverySlots;
using DeliveryService.Application.Slots.UpdateDeliverySlot;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Api.Controllers.External
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/delivery/slots")]
    public sealed class AdminDeliverySlotsController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminDeliverySlotsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetSlots(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAdminDeliverySlotsQuery(), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot(
            [FromBody] CreateDeliverySlotRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateDeliverySlotCommand(
                    request.ZoneId,
                    request.TimeFrom,
                    request.TimeTo,
                    request.Capacity),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{slotId:guid}")]
        public async Task<IActionResult> UpdateSlot(
            Guid slotId,
            [FromBody] UpdateDeliverySlotRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateDeliverySlotCommand(
                    slotId,
                    request.ZoneId,
                    request.TimeFrom,
                    request.TimeTo,
                    request.Capacity,
                    request.Status),
                cancellationToken);

            return result.ToActionResult();
        }
    }
}
