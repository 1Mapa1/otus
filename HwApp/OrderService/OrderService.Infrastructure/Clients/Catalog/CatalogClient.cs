using OrderService.Application.Abstractions.Clients.Catalog;
using OrderService.Infrastructure.Clients.Catalog.Requests;
using OrderService.Infrastructure.Clients.Catalog.Responses;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.Infrastructure.Clients.Catalog
{
    internal sealed class CatalogClient : ICatalogClient
    {
        private const string GetProductSnapshot = "api/internal/catalog/products/snapshot";

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetProductSnapshotResult> GetSnapshotAsync(
            IReadOnlyCollection<GetProductSnapshotItem> items,
            CancellationToken cancellationToken = default)
        {
            return await HttpClientTechnicalFailureHandler.ExecuteAsync(
                "CatalogService",
                async () =>
                {
                    var request = new GetProductSnapshotRequest(
                        items.Select(item => new ProductSnapshotItemDto(
                            item.ProductId,
                            item.Quantity,
                            item.ExpectedUnitPrice)).ToList());

                    var response = await _httpClient.PostAsJsonAsync(
                        GetProductSnapshot,
                        request,
                        Options,
                        cancellationToken);

                    HttpClientTechnicalFailureHandler.ThrowIfTechnicalFailure(
                        response,
                        "CatalogService",
                        "get product snapshot");

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var snapshot = JsonSerializer.Deserialize<GetProductSnapshotResponse>(content, Options);

                        if (snapshot is null)
                        {
                            return GetProductSnapshotResult.Failure(
                                new CatalogClientError(
                                    CatalogClientErrorCode.Unknown,
                                    "Catalog snapshot returned empty response."));
                        }

                        var resultItems = snapshot.Items
                            .Select(item => new CatalogSnapshotItem(
                                item.ProductId,
                                item.Name,
                                item.UnitPrice,
                                item.Quantity,
                                item.TotalPrice))
                            .ToList();

                        return GetProductSnapshotResult.Success(resultItems, snapshot.TotalAmount);
                    }

                    var errorResponse = JsonSerializer.Deserialize<CatalogErrorResponse>(content, Options);

                    return GetProductSnapshotResult.Failure(
                        ToCatalogError(response.StatusCode, errorResponse));
                });
        }

        private static CatalogClientError ToCatalogError(
            HttpStatusCode statusCode,
            CatalogErrorResponse? errorResponse)
        {
            var message = errorResponse?.Message;
            var errorCode = errorResponse?.Code ?? errorResponse?.ErrorCode;

            if (string.Equals(errorCode, "PriceChanged", StringComparison.OrdinalIgnoreCase))
            {
                var items = errorResponse?.Items?
                    .Select(item => new CatalogPriceChangedItem(
                        item.ProductId,
                        item.ExpectedUnitPrice,
                        item.ActualUnitPrice))
                    .ToList()
                    ?? [];

                return new CatalogClientError(
                    CatalogClientErrorCode.PriceChanged,
                    message,
                    items);
            }

            return statusCode switch
            {
                HttpStatusCode.BadRequest =>
                    new CatalogClientError(CatalogClientErrorCode.InvalidItems, message),

                HttpStatusCode.NotFound =>
                    new CatalogClientError(CatalogClientErrorCode.ProductNotFound, message),

                HttpStatusCode.Conflict =>
                    new CatalogClientError(CatalogClientErrorCode.ProductInactive, message),

                _ =>
                    new CatalogClientError(CatalogClientErrorCode.Unknown, message)
            };
        }
    }
}
