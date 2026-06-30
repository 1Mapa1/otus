using System.Text.Json;
using WarehouseService.Domain.Stocks;
using WarehouseService.Infrastructure.Persistence.Outbox;

namespace WarehouseService.Infrastructure.Messaging
{
    internal static class StockChangedOutboxFactory
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web);

        public static OutboxMessage Create(string topic, StockItem stockItem, DateTime occurredAt)
        {
            var eventId = Guid.NewGuid();

            var data = new
            {
                productId = stockItem.ProductId,
                availableQuantity = stockItem.AvailableQuantity,
                reservedQuantity = stockItem.ReservedQuantity,
                occurredAt
            };

            var envelope = new
            {
                EventId = eventId,
                EventType = "StockChanged",
                OccurredAt = occurredAt,
                Data = data
            };

            return new OutboxMessage
            {
                Id = eventId,
                Topic = topic,
                Key = stockItem.ProductId.ToString(),
                Payload = JsonSerializer.Serialize(envelope, JsonOptions),
                CreatedAt = occurredAt
            };
        }
    }
}
