using OrderService.Application.Abstractions.Clients.Warehouse;
using OrderService.Application.Abstractions.Clients.Warehouse.CancelReservation;
using OrderService.Application.Abstractions.Clients.Warehouse.CreateReservation;
using OrderService.Infrastructure.Clients.Warehouse.Dto;
using OrderService.Infrastructure.Clients.Warehouse.Requests;
using OrderService.Infrastructure.Clients.Warehouse.Responses;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.Infrastructure.Clients.Warehouse
{
    internal sealed class WarehouseClient : IWarehouseClient
    {
        private const string CreateReservation = "api/internal/warehouse/reservations";
        private const string CancelReservation = "api/internal/warehouse/reservations/cancel";

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public WarehouseClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CancelReservationResult> CancelReservationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await HttpClientTechnicalFailureHandler.ExecuteAsync(
                "WarehouseService",
                async () =>
                {
                    var request = new CancelReservationRequest(orderId);

                    var response = await _httpClient.PostAsJsonAsync(
                        CancelReservation,
                        request,
                        Options,
                        cancellationToken);

                    HttpClientTechnicalFailureHandler.ThrowIfTechnicalFailure(
                        response,
                        "WarehouseService",
                        "cancel reservation");

                    if (response.IsSuccessStatusCode)
                        return CancelReservationResult.Success();

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);

                    var errorResponse = JsonSerializer.Deserialize<WarehouseErrorResponse>(content, Options)!;

                    return CancelReservationResult.Failure(
                        ToWarehouseError(errorResponse));
                });
        }

        public async Task<CreateReservationResult> CreateReservationAsync(
            Guid orderId,
            Guid userId,
            IReadOnlyList<CreateReservationItem> products,
            CancellationToken cancellationToken = default)
        {
            return await HttpClientTechnicalFailureHandler.ExecuteAsync(
                "WarehouseService",
                async () =>
                {
                    var request = new CreateReservationRequest(
                        orderId,
                        userId,
                        products.Select(x => new ProductQuantityDto(x.ProductId, x.Quantity)));

                    var response = await _httpClient.PostAsJsonAsync(
                        CreateReservation,
                        request,
                        Options,
                        cancellationToken);

                    HttpClientTechnicalFailureHandler.ThrowIfTechnicalFailure(
                        response,
                        "WarehouseService",
                        "create reservation");

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var reservationResponse = JsonSerializer.Deserialize<CreateReservationResponse>(content, Options);

                        if (reservationResponse is null)
                        {
                            return CreateReservationResult.Failure(
                                new WarehouseClientError(
                                    WarehouseClientErrorCode.Unknown,
                                    "Warehouse reservation empty response."));
                        }

                        return CreateReservationResult.Success(reservationResponse.ReservationId);
                    }

                    var errorResponse = JsonSerializer.Deserialize<CreateReservationErrorResponses>(content, Options);

                    if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        return errorResponse?.ErrorCode switch
                        {
                            "StockNotAvailable" =>
                                CreateReservationResult.Failure(
                                    new WarehouseClientError(
                                        WarehouseClientErrorCode.StockNotAvailable,
                                        errorResponse?.ErrorMessage),
                                    errorResponse?.UnavailableItems?
                                        .Select(x => new UnavailableProductItem(
                                            x.ProductId,
                                            x.RequestedQuantity,
                                            x.FreeQuantity))
                                        .ToList()),

                            "InvalidReservationState" =>
                                CreateReservationResult.Failure(
                                    new WarehouseClientError(
                                        WarehouseClientErrorCode.InvalidReservationState,
                                        errorResponse?.ErrorMessage)),

                            _ =>
                                CreateReservationResult.Failure(
                                    new WarehouseClientError(
                                        WarehouseClientErrorCode.Unknown,
                                        errorResponse?.ErrorMessage))
                        };
                    }

                    return CreateReservationResult.Failure(ToWarehouseError(errorResponse));
                });
        }

        private static WarehouseClientError ToWarehouseError(WarehouseErrorResponse? errorResponse)
        {
            return new WarehouseClientError(
                WarehouseClientErrorCode.Unknown,
                errorResponse?.ErrorMessage);
        }
    }
}
