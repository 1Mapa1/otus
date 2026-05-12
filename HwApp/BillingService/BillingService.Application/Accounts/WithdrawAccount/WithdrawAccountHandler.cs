using MediatR;

namespace BillingService.Application.Accounts.WithdrawAccount
{
    internal sealed class WithdrawAccountHandler : IRequestHandler<WithdrawAccountCommand, WithdrawAccountResult>
    {
        private readonly IAccountRepository _accountRepository;

        public WithdrawAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<WithdrawAccountResult> Handle(WithdrawAccountCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                return WithdrawAccountResult.InvalidAmount();

            var result = await _accountRepository.TryWithdrawAsync(
                request.UserId,
                request.OrderId,
                request.Amount,
                cancellationToken);

            return result switch
            {
                WithdrawOperationResult.Success =>
                    WithdrawAccountResult.Success(),

                WithdrawOperationResult.InsufficientFunds =>
                    WithdrawAccountResult.InsufficientFunds(),

                WithdrawOperationResult.AccountNotFound =>
                    WithdrawAccountResult.AccountNotFound(),

                _ =>
                    throw new InvalidOperationException($"Unknown withdraw result: {result}")
            };
        }
    }
}
