using CatalogService.Domain.Events;

namespace CatalogService.Domain.Products.Events
{
    public sealed record ProductArchivedEvent(Guid productId) : IDomainEvent
    {
        public string Key => productId.ToString();
    }
}
