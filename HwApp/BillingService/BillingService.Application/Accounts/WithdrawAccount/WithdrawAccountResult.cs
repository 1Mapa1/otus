namespace BillingService.Application.Accounts.WithdrawAccount
{
    public sealed record WithdrawAccountResult(WithdrawAccountStatus Status)
    {
        public static WithdrawAccountResult Success()
        {
            return new WithdrawAccountResult(WithdrawAccountStatus.Success);
        }

        public static WithdrawAccountResult InvalidAmount()
        {
            return new WithdrawAccountResult(WithdrawAccountStatus.InvalidAmount);
        }

        public static WithdrawAccountResult AccountNotFound()
        {
            return new WithdrawAccountResult(WithdrawAccountStatus.AccountNotFound);
        }

        public static WithdrawAccountResult InsufficientFunds()
        {
            return new WithdrawAccountResult(WithdrawAccountStatus.InsufficientFunds);
        }
    }
}
