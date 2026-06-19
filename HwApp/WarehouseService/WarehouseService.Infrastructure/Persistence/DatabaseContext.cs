using Microsoft.EntityFrameworkCore;
using WarehouseService.Domain.Products;
using WarehouseService.Domain.StockReservations;

namespace WarehouseService.Infrastructure.Persistence
{
    internal sealed class DatabaseContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<StockReservation> StockReservations => Set<StockReservation>();
        public DbSet<StockReservationItem> StockReservationItems => Set<StockReservationItem>();

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
