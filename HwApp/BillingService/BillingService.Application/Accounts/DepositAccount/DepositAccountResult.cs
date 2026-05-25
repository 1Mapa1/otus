namespace BillingService.Application.Accounts.DepositAccount
{
    public sealed record DepositAccountResult(DepositAccountStatus Status)
    {
        public static DepositAccountResult Success()
        {
            return new DepositAccountResult(DepositAccountStatus.Success);
        }

        public static DepositAccountResult AccountNotFound()
        {
            return new DepositAccountResult(DepositAccountStatus.AccountNotFound);
        }

        public static DepositAccountResult InvalidAmount()
        {
            return new DepositAccountResult(DepositAccountStatus.InvalidAmount);
        }
    }
}