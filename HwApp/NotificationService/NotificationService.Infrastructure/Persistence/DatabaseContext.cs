using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Customers;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Persistence
{
    internal sealed class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) 
            : base(options)
        {
        }

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<NotificationCustomer> NotificationCustomers => Set<NotificationCustomer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        }
    }
}
