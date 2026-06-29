namespace BillingService.Infrastructure.Messaging
{
    internal sealed class DeadLetterMessageException : Exception
    {
        public DeadLetterMessageException(string message)
            : base(message)
        {
        }

        public DeadLetterMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
