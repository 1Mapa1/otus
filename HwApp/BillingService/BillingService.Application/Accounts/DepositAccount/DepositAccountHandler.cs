using MediatR;

namespace BillingService.Application.Accounts.DepositAccount
{
    internal sealed class DepositAccountHandler : IRequestHandler<DepositAccountCommand, DepositAccountResult>
    {
        private readonly IAccountRepository _accountRepository;

        public DepositAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<DepositAccountResult> Handle(DepositAccountCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                return DepositAccountResult.InvalidAmount();

            var result = await _accountRepository.DepositAsync(
                request.UserId,
                request.Amount,
                cancellationToken);

            return result switch
            {
                DepositOperationResult.Success =>
                    DepositAccountResult.Success(),

                DepositOperationResult.AccountNotFound =>
                    DepositAccountResult.AccountNotFound(),

                _ =>
                    throw new InvalidOperationException($"Unknown deposit result: {result}")
            };
        }
    }
}
