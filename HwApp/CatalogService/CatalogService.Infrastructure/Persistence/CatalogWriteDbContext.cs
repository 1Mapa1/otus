using System.Text.Json;
using CatalogService.Domain.Brands;
using CatalogService.Domain.Categories;
using CatalogService.Domain.Events;
using CatalogService.Domain.Products;
using CatalogService.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence
{
    internal sealed class CatalogWriteDbContext : DbContext
    {
        private readonly IIntegrationEventMapping _mapping;

        public CatalogWriteDbContext(DbContextOptions<CatalogWriteDbContext> options, IIntegrationEventMapping mapping)
            : base(options)
        {
            _mapping = mapping;
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();

        public DbSet<Brand> Brands => Set<Brand>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<ProductReadModel> ProductReadModels => Set<ProductReadModel>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogWriteDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker
                .Entries<IHasDomainEvents>()
                .SelectMany(x => x.Entity.Events)
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                var @event = _mapping.Resolve(domainEvent);

                var eventData = JsonSerializer.SerializeToElement(
                    domainEvent,
                    domainEvent.GetType());

                var envelope = new
                {
                    EventId = Guid.NewGuid(),
                    @event.EventType,
                    OccurredAt = DateTime.UtcNow,
                    Data = eventData
                };

                OutboxMessages.Add(new OutboxMessage
                {
                    Id = envelope.EventId,
                    Topic = @event.Topic,
                    Key = @event.Key,
                    Payload = JsonSerializer.Serialize(envelope),
                    CreatedAt = envelope.OccurredAt
                });
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var entry in ChangeTracker.Entries<IHasDomainEvents>())
            {
                entry.Entity.ClearEvents();
            }

            return result;
        }
    }
}
