using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.Outbox;

namespace OrderService.Infrastructure.Persistence
{
    internal sealed class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionStringLocal();

            optionsBuilder.UseNpgsql(connectionString);

            return new DatabaseContext(
                optionsBuilder.Options,
                new IntegrationEventMapping());
        }
    }
}
