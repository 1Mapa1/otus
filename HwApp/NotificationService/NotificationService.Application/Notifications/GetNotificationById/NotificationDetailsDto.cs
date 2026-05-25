namespace NotificationService.Application.Notifications.GetNotificationById
{
    public sealed record NotificationDetailsDto(
        Guid Id,
        string? Email,
        string? CustomerName,
        Guid? OrderId,
        string Type,
        string Subject,
        string Body,
        DateTime CreatedAt);
}
