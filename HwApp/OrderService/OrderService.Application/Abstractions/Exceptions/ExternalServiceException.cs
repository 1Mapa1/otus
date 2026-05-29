namespace OrderService.Application.Abstractions.Exceptions
{
    public sealed class ExternalServiceException : Exception
    {
        public ExternalServiceException(string serviceName, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            ServiceName = serviceName;
        }

        public string ServiceName { get; }
    }
}
