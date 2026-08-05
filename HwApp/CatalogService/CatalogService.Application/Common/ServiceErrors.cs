namespace CatalogService.Application.Common
{
    public static class ServiceErrors
    {
        public static readonly Error ServiceUnavailable = new(
            "ServiceUnavailable",
            "The service is temporarily unavailable.",
            ErrorType.Failure);
    }
}
