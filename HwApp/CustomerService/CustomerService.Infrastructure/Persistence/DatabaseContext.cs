using Microsoft.EntityFrameworkCore;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using System.Text.Json;
using CustomerService.Infrastructure.Persistence.Outbox;

namespace CustomerService.Infrastructure.Persistence
{
    internal class DatabaseContext : DbContext
    {
        public const string CONNECTION_NAME = "Npgsql";

        private readonly IIntegrationEventMapping _mapping;

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IIntegrationEventMapping mapping) 
            : base(options)
        {
            _mapping = mapping;
        }

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

                var envelope = new
                {
                    EventId = Guid.NewGuid(),
                    @event.EventType,
                    OccuredAt = DateTime.UtcNow,
                    Data = domainEvent
                };

                OutboxMessages.Add(new OutboxMessage
                {
                    Id = envelope.EventId,
                    Topic = @event.Topic,
                    Payload = JsonSerializer.Serialize(envelope),
                    CreatedAt = envelope.OccuredAt
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
