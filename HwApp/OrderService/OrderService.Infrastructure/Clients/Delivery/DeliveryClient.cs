using OrderService.Application.Abstractions.Delivery;
using OrderService.Application.Abstractions.Delivery.CancelReservation;
using OrderService.Application.Abstractions.Delivery.CreateReservation;
using OrderService.Infrastructure.Clients.Delivery.Requests;
using OrderService.Infrastructure.Clients.Delivery.Responses;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.Infrastructure.Clients.Delivery
{
    internal sealed class DeliveryClient : IDeliveryClient
    {
        private const string CreateReservation = "api/internal/delivery/reservations";
        private const string CancelReservation = "api/internal/delivery/reservations/cancel";

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public DeliveryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CancelReservationResult> CancelReservationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var request = new CancelReservationRequest(orderId);

            var response = await _httpClient.PostAsJsonAsync(
                CancelReservation,
                request,
                Options,
                cancellationToken);

            if (response.IsSuccessStatusCode)
                return CancelReservationResult.Success();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var errorResponse = JsonSerializer.Deserialize<DeliveryErrorResponse>(content, Options);

            return CancelReservationResult.Failure(
                ToDeliveryError(response.StatusCode, errorResponse));
        }

        public async Task<CreateReservationResult> CreateReservationAsync(
            Guid orderId,
            Guid userId,
            Guid deliverySlotId,
            CancellationToken cancellationToken = default)
        {
            var request = new CreateReservationRequest(orderId, userId, deliverySlotId);

            var response = await _httpClient.PostAsJsonAsync(
                CreateReservation,
                request,
                Options,
                cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var createReservationResponse = JsonSerializer.Deserialize<CreateReservationResponse>(content, Options);

                if (createReservationResponse is null)
                    return CreateReservationResult.Failure(
                        new DeliveryClientError(DeliveryClientErrorCode.Unknown, "Delivery reservation empty response."));

                return CreateReservationResult.Success(createReservationResponse.ReservationId);
            }

            var errorResponse = JsonSerializer.Deserialize<DeliveryErrorResponse>(content, Options);

            return CreateReservationResult.Failure(
                ToDeliveryError(response.StatusCode, errorResponse));
        }

        private static DeliveryClientError ToDeliveryError(
            HttpStatusCode statusCode,
            DeliveryErrorResponse? errorResponse)
        {
            var message = errorResponse?.ErrorMessage;

            if (statusCode == HttpStatusCode.Conflict)
            {
                return errorResponse?.ErrorCode switch
                {
                    "DeliverySlotUnavailable" =>
                        new DeliveryClientError(DeliveryClientErrorCode.SlotNotAvailable, message),

                    "InvalidReservationState" =>
                        new DeliveryClientError(DeliveryClientErrorCode.InvalidReservationState, message),

                    _ =>
                        new DeliveryClientError(DeliveryClientErrorCode.Unknown, message)
                };
            }


            return new DeliveryClientError(DeliveryClientErrorCode.Unknown, message);
        }
    }
}
