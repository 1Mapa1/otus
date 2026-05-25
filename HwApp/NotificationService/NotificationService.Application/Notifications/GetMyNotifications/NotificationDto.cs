namespace NotificationService.Application.Notifications.GetMyNotifications
{
    public sealed record NotificationDto(
        Guid Id,
        Guid? OrderId,
        string Type,
        string Subject,
        DateTime CreatedAt);
}
