namespace CatalogService.Application.Common
{
    public static class ReadResults
    {
        public static async Task<Result<T>> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> action,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Result<T>.Success(await action(cancellationToken));
            }
            catch (ServiceUnavailableException)
            {
                return Result<T>.Failure(ServiceErrors.ServiceUnavailable);
            }
            catch (Exception ex) when (IsConnectionFailure(ex))
            {
                return Result<T>.Failure(ServiceErrors.ServiceUnavailable);
            }
        }

        private static bool IsConnectionFailure(Exception exception)
        {
            return exception switch
            {
                TimeoutException => true,
                _ when string.Equals(exception.GetType().Name, "NpgsqlException", StringComparison.Ordinal) => true,
                _ => exception.InnerException is not null && IsConnectionFailure(exception.InnerException)
            };
        }
    }
}
