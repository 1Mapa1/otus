using CatalogService.Domain.Events;

namespace CatalogService.Domain.Products.Events
{
    public sealed record ProductRestoredEvent(Guid productId) : IDomainEvent
    {
        public string Key => productId.ToString();
    }
}
