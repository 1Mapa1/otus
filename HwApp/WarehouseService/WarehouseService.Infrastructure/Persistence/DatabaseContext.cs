using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WarehouseService.Domain.Events;
using WarehouseService.Domain.StockReservations;
using WarehouseService.Domain.Stocks;
using WarehouseService.Infrastructure.Persistence.Outbox;

namespace WarehouseService.Infrastructure.Persistence
{
    internal sealed class DatabaseContext : DbContext
    {
        private readonly IIntegrationEventMapping _mapping;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IIntegrationEventMapping mapping)
            : base(options)
        {
            _mapping = mapping;
        }

        public DbSet<StockItem> StockItems => Set<StockItem>();
        public DbSet<StockReservation> StockReservations => Set<StockReservation>();
        public DbSet<StockReservationItem> StockReservationItems => Set<StockReservationItem>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
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
