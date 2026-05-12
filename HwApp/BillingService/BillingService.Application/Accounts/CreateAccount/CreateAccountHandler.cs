using BillingService.Application.Abstractions;
using BillingService.Domain.Accounts;
using MediatR;

namespace BillingService.Application.Accounts.CreateAccount
{
    internal sealed class CreateAccountHandler : IRequestHandler<CreateAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;

        public CreateAccountHandler(IUnitOfWork unitOfWork, IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
        }

        public async Task Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (existingAccount is not null)
                return;

            var account = Account.Create(request.UserId);

            await _accountRepository.AddAsync(account, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
