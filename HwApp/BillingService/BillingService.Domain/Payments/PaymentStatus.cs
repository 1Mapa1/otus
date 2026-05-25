namespace BillingService.Domain.Payments
{
    public enum PaymentStatus
    {
        Authorized = 0,
        Captured = 1,
        AuthorizationCanceled = 2
    }
}
