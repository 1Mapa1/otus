using OrderService.Application.Abstractions.Clients.Billing;
using OrderService.Application.Abstractions.Clients.Billing.AuthorizePayment;
using OrderService.Application.Abstractions.Clients.Billing.CancelAuthorization;
using OrderService.Application.Abstractions.Clients.Billing.CapturePayment;
using OrderService.Infrastructure.Clients.Billing.Requests;
using OrderService.Infrastructure.Clients.Billing.Responses;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.Infrastructure.Clients.Billing
{
    internal sealed class BillingClient : IBillingClient
    {
        private const string AuthorizePaymentEndpoint = "api/internal/billing/payments/authorize";
        private const string CapturePaymentEndpoint = "api/internal/billing/payments/capture";
        private const string CancelAuthorizationEndpoint = "api/internal/billing/payments/cancel-authorization";

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public BillingClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthorizePaymentResult> AuthorizePaymentAsync(
            Guid userId,
            Guid orderId,
            decimal amount,
            CancellationToken cancellationToken = default)
        {
            return await HttpClientTechnicalFailureHandler.ExecuteAsync(
                "BillingService",
                async () =>
                {
                    var request = new AuthorizePaymentRequest(userId, orderId, amount);

                    var response = await _httpClient.PostAsJsonAsync(
                        AuthorizePaymentEndpoint,
                        request,
                        Options,
                        cancellationToken);

                    HttpClientTechnicalFailureHandler.ThrowIfTechnicalFailure(response, "BillingService", "authorize payment");

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var body = JsonSerializer.Deserialize<AuthorizePaymentResponse>(content, Options);

                        if (body is null)
                        {
                            return AuthorizePaymentResult.Failure(
                                new BillingClientError(BillingClientErrorCode.Unknown, "Billing authorize empty response."));
                        }

                        return AuthorizePaymentResult.Success(body.PaymentId, body.AuthorizedAmount);
                    }

                    var errorResponse = JsonSerializer.Deserialize<BillingErrorResponse>(content, Options);

                    return AuthorizePaymentResult.Failure(
                        ToBillingError(response.StatusCode, errorResponse, isAuthorize: true));
                });
        }

        public async Task<CapturePaymentResult> CapturePaymentAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await HttpClientTechnicalFailureHandler.ExecuteAsync(
                "BillingService",
                async () =>
                {
                    var request = new CapturePaymentRequest(orderId);

                    var response = await _httpClient.PostAsJsonAsync(
                        CapturePaymentEndpoint,
                        request,
                        Options,
                        cancellationToken);

                    HttpClientTechnicalFailureHandler.ThrowIfTechnicalFailure(response, "BillingService", "capture payment");

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var body = JsonSerializer.Deserialize<CapturePaymentResponse>(content, Options);

                        if (body is null)
                        {
                            return CapturePaymentResult.Failure(
                                new BillingClientError(BillingClientErrorCode.Unknown, "Billing capture empty response."));
                        }

                        return CapturePaymentResult.Success(body.PaymentId, body.AuthorizedAmount);
                    }

                    var errorResponse = JsonSerializer.Deserialize<BillingErrorResponse>(content, Options);

                    return CapturePaymentResult.Failure(
                        ToBillingError(response.StatusCode, errorResponse, isAuthorize: false));
                });
        }

        public async Task<CancelAuthorizationResult> CancelAuthorizationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await HttpClientTechnicalFailureHandler.ExecuteAsync(
                "BillingService",
                async () =>
                {
                    var request = new CancelAuthorizationPaymentRequest(orderId);

                    var response = await _httpClient.PostAsJsonAsync(
                        CancelAuthorizationEndpoint,
                        request,
                        Options,
                        cancellationToken);

                    HttpClientTechnicalFailureHandler.ThrowIfTechnicalFailure(response, "BillingService", "cancel authorication");

                    if (response.IsSuccessStatusCode)
                        return CancelAuthorizationResult.Success();

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var errorResponse = JsonSerializer.Deserialize<BillingErrorResponse>(content, Options);

                    return CancelAuthorizationResult.Failure(
                        ToBillingError(response.StatusCode, errorResponse, isAuthorize: false));
                });
        }

        private static BillingClientError ToBillingError(
            HttpStatusCode statusCode,
            BillingErrorResponse? errorResponse,
            bool isAuthorize)
        {
            var errorCode = errorResponse?.ErrorCode;

            if (statusCode == HttpStatusCode.NotFound)
            {
                return errorCode switch
                {
                    "PaymentNotFound" =>
                        new BillingClientError(BillingClientErrorCode.PaymentNotFound, errorCode),

                    "AccountNotFound" =>
                        new BillingClientError(BillingClientErrorCode.AccountNotFound, errorCode),

                    _ =>
                        new BillingClientError(
                            isAuthorize ? BillingClientErrorCode.AccountNotFound : BillingClientErrorCode.PaymentNotFound,
                            errorCode)
                };
            }

            if (statusCode == HttpStatusCode.BadRequest)
            {
                return new BillingClientError(BillingClientErrorCode.InvalidAmount, errorCode);
            }

            if (statusCode == HttpStatusCode.Conflict)
            {
                return errorCode switch
                {
                    "InsufficientFunds" =>
                        new BillingClientError(BillingClientErrorCode.InsufficientFunds, errorCode),

                    "InvalidPaymentState" =>
                        new BillingClientError(BillingClientErrorCode.InvalidPaymentState, errorCode),

                    "AccountStateConflict" =>
                        new BillingClientError(BillingClientErrorCode.AccountStateConflict, errorCode),

                    _ =>
                        new BillingClientError(BillingClientErrorCode.Unknown, errorCode)
                };
            }

            return new BillingClientError(BillingClientErrorCode.Unknown, errorCode);
        }
    }
}
