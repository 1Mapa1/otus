using OrderService.Application.Abstractions.Exceptions;
using System.Net;
using System.Text.Json;

namespace OrderService.Infrastructure.Clients
{
    internal static class HttpClientTechnicalFailureHandler
    {
        public static void ThrowIfTechnicalFailure(
            HttpResponseMessage response,
            string serviceName,
            string operation)
        {
            if ((int)response.StatusCode >= 500 ||
                response.StatusCode == HttpStatusCode.RequestTimeout ||
                response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new ExternalServiceException(
                    serviceName,
                    $"{serviceName} {operation} failed with status code {(int)response.StatusCode}.");
            }
        }

        public static async Task<T> ExecuteAsync<T>(
            string serviceName,
            Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (TaskCanceledException ex)
            {
                throw new ExternalServiceException(
                    serviceName,
                    $"{serviceName} request timed out.",
                    ex);
            }
            catch (HttpRequestException ex)
            {
                throw new ExternalServiceException(
                    serviceName,
                    $"{serviceName} request failed.",
                    ex);
            }
            catch (JsonException ex)
            {
                throw new ExternalServiceException(
                    serviceName,
                    $"{serviceName} returned invalid JSON.",
                    ex);
            }
        }
    }
}
