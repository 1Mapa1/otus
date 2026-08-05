using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AuthService.Infrastructure.Rersistence
{
    internal class AuthDbContext : DbContext
    {
        private readonly IIntegrationEventMapping _mapping;

        public DbSet<User> Users => Set<User>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public AuthDbContext(
            DbContextOptions<AuthDbContext> options,
            IIntegrationEventMapping mapping)
            : base(options)
        {
            _mapping = mapping;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

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
