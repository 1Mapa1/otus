using Microsoft.EntityFrameworkCore;
using CustomerService.Domain.Entities;

namespace CustomerService.Infrastructure.Ef
{
    internal class DatabaseContext : DbContext
    {
        public const string CONNECTION_NAME = "Npgsql";

        public DbSet<Customer> Customers => Set<Customer>();

        public DatabaseContext(DbContextOptions<DatabaseContext> options) 
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
