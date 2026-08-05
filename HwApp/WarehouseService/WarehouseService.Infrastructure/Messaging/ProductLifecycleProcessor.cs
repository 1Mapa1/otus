using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseService.Domain.Stocks;
using WarehouseService.Infrastructure.Messaging.Contracts;
using WarehouseService.Infrastructure.Persistence;

namespace WarehouseService.Infrastructure.Messaging
{
    internal sealed class ProductLifecycleProcessor
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

        private static readonly HashSet<string> SupportedEventTypes = new(StringComparer.Ordinal)
        {
            "product.created.v1",
            "product.archived.v1",
            "product.restored.v1"
        };

        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ProductLifecycleProcessor> _logger;

        public ProductLifecycleProcessor(
            DatabaseContext databaseContext,
            ILogger<ProductLifecycleProcessor> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        public async Task ProcessAsync(string messageValue, CancellationToken cancellationToken)
        {
            var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope>(messageValue, JsonOptions);

            if (envelope is null || string.IsNullOrWhiteSpace(envelope.EventType))
                throw new InvalidOperationException("Kafka message envelope is invalid.");

            if (!SupportedEventTypes.Contains(envelope.EventType))
            {
                _logger.LogWarning(
                    "Skipping unsupported catalog event type: {EventType}",
                    envelope.EventType);
                return;
            }

            var productId = envelope.Data.GetProperty("productId").GetGuid();

            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            switch (envelope.EventType)
            {
                case "product.created.v1":
                    await HandleProductCreatedAsync(productId, cancellationToken);
                    break;

                case "product.archived.v1":
                    await HandleProductArchivedAsync(productId, cancellationToken);
                    break;

                case "product.restored.v1":
                    await HandleProductRestoredAsync(productId, cancellationToken);
                    break;
            }

            await _databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        private async Task HandleProductCreatedAsync(Guid productId, CancellationToken cancellationToken)
        {
            var exists = await _databaseContext.StockItems
                .AsNoTracking()
                .AnyAsync(item => item.ProductId == productId, cancellationToken);

            if (exists)
                return;

            await _databaseContext.StockItems.AddAsync(StockItem.Create(productId), cancellationToken);
        }

        private async Task HandleProductArchivedAsync(Guid productId, CancellationToken cancellationToken)
        {
            var stockItem = await _databaseContext.StockItems
                .FirstOrDefaultAsync(item => item.ProductId == productId, cancellationToken);

            if (stockItem is null)
            {
                _logger.LogWarning(
                    "ProductArchived received for unknown stock item. ProductId: {ProductId}",
                    productId);
                return;
            }

            stockItem.Archive();
        }

        private async Task HandleProductRestoredAsync(Guid productId, CancellationToken cancellationToken)
        {
            var stockItem = await _databaseContext.StockItems
                .FirstOrDefaultAsync(item => item.ProductId == productId, cancellationToken);

            if (stockItem is null)
            {
                _logger.LogWarning(
                    "ProductRestored received for unknown stock item. ProductId: {ProductId}",
                    productId);
                return;
            }

            stockItem.Restore();
        }
    }
}
