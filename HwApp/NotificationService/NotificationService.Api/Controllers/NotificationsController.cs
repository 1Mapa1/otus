using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Contracts.Notifications;
using NotificationService.Application.Notifications.GetMyNotifications;
using NotificationService.Application.Notifications.GetNotificationById;
using System.Security.Claims;

namespace NotificationService.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly ISender _sender;

        public NotificationsController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetMyNotifications(CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            IReadOnlyList<NotificationDto> notifications = await _sender.Send(new GetMyNotificationsQuery(userId), ct);

            return Ok(notifications
                .Select(n => new NotificationResponse(
                    n.Id,
                    n.OrderId,
                    n.Type,
                    n.Subject,
                    n.CreatedAt)));
        }

        [Authorize]
        [HttpGet("{notificationId:guid}")]
        public async Task<ActionResult<NotificationDetailsResponse>> GetById(Guid notificationId, CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            NotificationDetailsDto? notification = await _sender.Send(new GetNotificationByIdQuery(userId, notificationId), ct);

            if (notification is null)
                return NotFound();

            return Ok(new NotificationDetailsResponse(
                notification.Id,
                userId,
                notification.Email,
                notification.CustomerName,
                notification.OrderId,
                notification.Type,
                notification.Subject,
                notification.Body,
                notification.CreatedAt));
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            var idClaim = User.FindFirstValue("sub")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.Identity?.Name;

            return Guid.TryParse(idClaim, out userId);
        }
    }
}
