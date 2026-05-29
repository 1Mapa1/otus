namespace OrderService.Application.Abstractions.Clients.Billing
{
    public enum BillingClientErrorCode
    {
        AccountNotFound = 1,
        InsufficientFunds = 2,
        InvalidAmount = 3,
        InvalidPaymentState = 4,
        PaymentNotFound = 5,
        AccountStateConflict = 6,
        Unknown = 100
    }
}
