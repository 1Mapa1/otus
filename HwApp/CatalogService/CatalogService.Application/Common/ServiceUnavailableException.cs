namespace CatalogService.Application.Common
{
    public sealed class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException()
            : base("The service is temporarily unavailable.")
        {
        }
    }
}
