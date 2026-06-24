using AuthService.Infrastructure.Persistence.Outbox;
using AuthService.Infrastructure.Rersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Rersistence
{
    internal sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("Npgsql")
                ?? "Host=localhost;Database=auth_db;Username=auth_user;Password=auth_pass";

            optionsBuilder.UseNpgsql(connectionString);

            return new AuthDbContext(
                optionsBuilder.Options,
                new IntegrationEventMapping());
        }
    }
}
