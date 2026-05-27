using DeliveryService.Application.Abstractions;
using DeliveryService.Application.Reservations;
using DeliveryService.Application.Slots;
using DeliveryService.Infrastructure.Persistence;
using DeliveryService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddInfrastructureDatabaseContext(configuration);
            services.AddInfrastructureRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IHealthChecksBuilder AddInfrastructureHealthChecks(
            this IServiceCollection services,
            string healthCheckName = "Database",
            IEnumerable<string>? healthCheckTags = null)
        {
            healthCheckTags ??= ["ready", "startup"];

            return services.AddHealthChecks()
                .AddDbContextCheck<DatabaseContext>(
                    name: healthCheckName,
                    tags: healthCheckTags);
        }

        public static IServiceCollection AddInfrastructureDatabaseContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionStringLocal();

            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        private static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDeliverySlotRepository, DeliverySlotRepository>();
            services.AddScoped<IDeliveryReservationRepository, DeliveryReservationRepository>();

            return services;
        }

        internal static string GetConnectionStringLocal(this IConfiguration configuration)
        {
            var connectionString = !string.IsNullOrEmpty(configuration["DB_HOST"])
                ? $"Host={configuration["DB_HOST"]};" +
                  $"Port={configuration["DB_PORT"]};" +
                  $"Database={configuration["DB_NAME"]};" +
                  $"Username={configuration["DB_USER"]};" +
                  $"Password={configuration["DB_PASSWORD"]}"
                : configuration.GetConnectionString("Npgsql");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string env or appsettings Npgsql was not found.");

            return connectionString;
        }

        public static async Task MigrationAsync(this IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            await db.Database.MigrateAsync();
        }
    }
}
