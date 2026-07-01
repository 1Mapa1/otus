using System.Text.Json;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Domain.Products;
using CatalogService.Infrastructure.Messaging;
using CatalogService.Infrastructure.Messaging.Kafka;
using CatalogService.Infrastructure.Options;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CatalogService.Infrastructure.Messaging
{
    internal sealed class StockChangedProcessor
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

        private readonly CatalogWriteDbContext _databaseContext;
        private readonly CatalogOptions _catalogOptions;
        private readonly ILogger<StockChangedProcessor> _logger;

        public StockChangedProcessor(
            CatalogWriteDbContext databaseContext,
            IOptions<CatalogOptions> catalogOptions,
            ILogger<StockChangedProcessor> logger)
        {
            _databaseContext = databaseContext;
            _catalogOptions = catalogOptions.Value;
            _logger = logger;
        }

        public async Task ProcessAsync(string messageValue, CancellationToken cancellationToken)
        {
            var envelope = JsonSerializer.Deserialize<Contracts.IntegrationEventEnvelope>(messageValue, JsonOptions);

            if (envelope is null || string.IsNullOrWhiteSpace(envelope.EventType))
                throw new InvalidOperationException("Kafka message envelope is invalid.");

            if (!string.Equals(envelope.EventType, "StockChanged", StringComparison.Ordinal))
            {
                _logger.LogWarning("Skipping unsupported warehouse event type: {EventType}", envelope.EventType);
                return;
            }

            var productId = envelope.Data.GetProperty("productId").GetGuid();
            var availableQuantity = envelope.Data.GetProperty("availableQuantity").GetInt32();
            var status = availableQuantity <= 0
                ? AvailabilityStatus.OutOfStock
                : availableQuantity <= _catalogOptions.LowStockThreshold
                    ? AvailabilityStatus.LowStock
                    : AvailabilityStatus.InStock;
            var utcNow = DateTime.UtcNow;

            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var readModel = await _databaseContext.ProductReadModels
                .FirstOrDefaultAsync(model => model.ProductId == productId, cancellationToken);

            if (readModel is null)
            {
                _logger.LogWarning(
                    "StockChanged received for unknown product read model. ProductId: {ProductId}",
                    productId);
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            readModel.SetAvailability(status, utcNow);
            await _databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
