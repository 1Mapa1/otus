using DeliveryService.Domain.Reservations;
using DeliveryService.Domain.Slots;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Infrastructure.Persistence
{
    internal sealed class DatabaseContext : DbContext
    {
        
        public DbSet<DeliverySlot> DeliverySlots => Set<DeliverySlot>();
        public DbSet<DeliveryReservation> DeliveryReservations => Set<DeliveryReservation>();

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        }
    }
}
