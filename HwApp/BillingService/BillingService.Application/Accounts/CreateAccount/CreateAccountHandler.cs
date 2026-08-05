using MediatR;

namespace BillingService.Application.Accounts.CreateAccount
{
    internal sealed class CreateAccountHandler : IRequestHandler<CreateAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            await _accountRepository.EnsureAccountAsync(
                request.UserId,
                cancellationToken);
        }
    }
}
