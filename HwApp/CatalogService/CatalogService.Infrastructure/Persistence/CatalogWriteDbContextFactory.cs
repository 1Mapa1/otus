using CatalogService.Infrastructure.Persistence;
using CatalogService.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CatalogService.Infrastructure.Persistence
{
    internal sealed class CatalogWriteDbContextFactory : IDesignTimeDbContextFactory<CatalogWriteDbContext>
    {
        public CatalogWriteDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("CatalogPrimary")
                ?? "Host=localhost;Port=5432;Database=catalog;Username=postgres;Password=postgres";

            var optionsBuilder = new DbContextOptionsBuilder<CatalogWriteDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new CatalogWriteDbContext(
                optionsBuilder.Options,
                new IntegrationEventMapping());
        }
    }
}
