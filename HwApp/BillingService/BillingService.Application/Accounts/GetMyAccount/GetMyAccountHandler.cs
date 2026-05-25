using MediatR;

namespace BillingService.Application.Accounts.GetMyAccount
{
    internal sealed class GetMyAccountHandler : IRequestHandler<GetMyAccountQuery, AccountDto?>
    {
        private readonly IAccountRepository _accountRepository;

        public GetMyAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDto?> Handle(GetMyAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (account is null)
                return null;

            return new AccountDto(account.UserId, account.Balance, account.CreatedAt);
        }
    }
}
