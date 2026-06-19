using BillingService.Domain.Accounts;
using BillingService.Domain.AccountTransactions;
using BillingService.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infrastructure.Persistence
{
    internal sealed class DatabaseContext : DbContext
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<AccountTransaction> AccountTransactions => Set<AccountTransaction>();
        public DbSet<Payment> Payments => Set<Payment>();

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