namespace NotificationService.Api.Contracts.Notifications
{
    public sealed record NotificationDetailsResponse(
        Guid Id,
        Guid UserId,
        string? Email,
        string? CustomerName,
        Guid? OrderId,
        string Type,
        string Subject,
        string Body,
        DateTime CreatedAt);
}
