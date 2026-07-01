using WarehouseService.Domain.Events;

namespace WarehouseService.Domain.Stocks.Events
{
    public sealed record StockChangedEvent(
        Guid productId,
        int availableQuantity,
        int reservedQuantity,
        DateTime occurredAt) : IDomainEvent
    {
        public string Key => productId.ToString();
    }
}
