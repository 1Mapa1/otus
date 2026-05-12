using MediatR;

namespace BillingService.Application.Accounts.GetMyAccount
{
    public sealed record GetMyAccountQuery(
        Guid UserId) : IRequest<AccountDto?>;
}
