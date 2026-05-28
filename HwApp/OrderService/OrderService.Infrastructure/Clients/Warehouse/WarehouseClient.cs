using Confluent.Kafka;
using OrderService.Application.Abstractions.Delivery;
using OrderService.Application.Abstractions.Warehouse;
using OrderService.Application.Abstractions.Warehouse.CancelReservation;
using OrderService.Application.Abstractions.Warehouse.CreateReservation;
using OrderService.Application.Abstractions.Warehouse.ResolveProducts;
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
        private const string ResolveProducts = "api/internal/warehouse/products/resolve";
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

        public async Task<CancelReservationResult> CancelReservationAsync(Guid orderId, CancellationToken cancellationToken = default)
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

            var errorResponse = JsonSerializer.Deserialize<WarehouseErrorResponse>(content, Options)!;

            return CancelReservationResult.Failure(
                ToWarehouseError(response.StatusCode, errorResponse));
        }

        public async Task<CreateReservationResult> CreateReservationAsync(Guid orderId, Guid userId, IReadOnlyList<CreateReservationItem> products, CancellationToken cancellationToken = default)
        {
            var request = new CreateReservationRequest(orderId, userId, products.Select(x => new ProductQuantityDto(x.ProductId, x.Quantity)));

            var response = await _httpClient.PostAsJsonAsync(
                CreateReservation,
                request,
                Options,
                cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var resolvedProducts = JsonSerializer.Deserialize<CreateReservationResponse>(content, Options);

                if (resolvedProducts is null)
                    return CreateReservationResult.Failure(new WarehouseClientError(WarehouseClientErrorCode.Unknown, "Warehouse reservation empty response."));

                return CreateReservationResult.Success(resolvedProducts.ReservationId);
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
                                .Select(x => new UnavailableProductItem(x.ProductId, x.RequestedQuantity, x.FreeQuantity)).ToList()),

                    "InvalidReservationState" =>
                        CreateReservationResult.Failure(
                            new WarehouseClientError(
                                WarehouseClientErrorCode.InvalidReservationState,
                                errorResponse?.ErrorMessage)),

                    _ =>
                        CreateReservationResult.Failure(
                            new WarehouseClientError(WarehouseClientErrorCode.Unknown, errorResponse?.ErrorMessage))
                };
            }

            return CreateReservationResult.Failure(
                ToWarehouseError(response.StatusCode, errorResponse));
        }

        public async Task<ResolveProductsResult> ResolveProductsAsync(IReadOnlyList<ResolveProductItem> products, CancellationToken cancellationToken = default)
        {
            var request = new ResolveProductsRequest(products.Select(x => new ProductQuantityDto(x.ProductId, x.Quantity)));

            var response = await _httpClient.PostAsJsonAsync(
                ResolveProducts,
                request,
                Options,
                cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var resolvedProducts = JsonSerializer.Deserialize<ResolveProductsResponse>(content, Options);

                if (resolvedProducts is null)
                    return ResolveProductsResult.Failure(new WarehouseClientError(WarehouseClientErrorCode.Unknown, "Warehouse resolve returned empty response."));

                var resultItems = resolvedProducts.Items
                    .Select(x => new ResolvedProductItem(x.ProductId, x.Name, x.UnitPrice, x.Quantity, x.TotalPrice)).ToList();

                return ResolveProductsResult.Success(resultItems, resolvedProducts.TotalAmount);
            }

            var errorResponse = JsonSerializer.Deserialize<WarehouseErrorResponse>(content, Options);

            return ResolveProductsResult.Failure(
                ToWarehouseError(response.StatusCode, errorResponse));
        }

        private static WarehouseClientError ToWarehouseError(
            HttpStatusCode statusCode,
            WarehouseErrorResponse? errorResponse)
        {
            var message = errorResponse?.ErrorMessage;

            return statusCode switch
            {
                HttpStatusCode.BadRequest =>
                    new WarehouseClientError(WarehouseClientErrorCode.InvalidItems, message),

                HttpStatusCode.NotFound =>
                    new WarehouseClientError(WarehouseClientErrorCode.ProductNotFound, message),

                HttpStatusCode.Conflict =>
                    new WarehouseClientError(WarehouseClientErrorCode.StockNotAvailable, message),

                _ =>
                    new WarehouseClientError(WarehouseClientErrorCode.Unknown, message)
            };
        }
    }
}
