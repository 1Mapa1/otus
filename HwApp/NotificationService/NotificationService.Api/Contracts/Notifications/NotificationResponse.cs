namespace NotificationService.Api.Contracts.Notifications
{
    public sealed record NotificationResponse(
        Guid Id,
        Guid? OrderId,
        string Type,
        string Subject,
        DateTime CreatedAt);
}
