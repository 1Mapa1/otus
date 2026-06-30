using Microsoft.EntityFrameworkCore;
using WarehouseService.Domain.StockReservations;
using WarehouseService.Domain.Stocks;
using WarehouseService.Infrastructure.Persistence.Outbox;

namespace WarehouseService.Infrastructure.Persistence
{
    internal sealed class DatabaseContext : DbContext
    {
        public DbSet<StockItem> StockItems => Set<StockItem>();
        public DbSet<StockReservation> StockReservations => Set<StockReservation>();
        public DbSet<StockReservationItem> StockReservationItems => Set<StockReservationItem>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

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
