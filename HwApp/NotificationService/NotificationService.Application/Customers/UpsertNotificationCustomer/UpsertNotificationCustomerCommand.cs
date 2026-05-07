using MediatR;

namespace NotificationService.Application.Customers.UpsertNotificationCustomer
{
    public sealed record UpsertNotificationCustomerCommand(
        Guid UserId,
        string Name,
        string Email) : IRequest;
}
