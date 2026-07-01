using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseService.Application.Abstractions;
using WarehouseService.Application.Reservations;
using WarehouseService.Application.Stocks;
using WarehouseService.Infrastructure.Messaging;
using WarehouseService.Infrastructure.Messaging.Kafka;
using WarehouseService.Infrastructure.Persistence;
using WarehouseService.Infrastructure.Persistence.Outbox;
using WarehouseService.Infrastructure.Persistence.Repositories;
using WarehouseService.Infrastructure.Workers;

namespace WarehouseService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IIntegrationEventMapping, IntegrationEventMapping>();

            services.AddInfrastructureDatabaseContext(configuration);
            services.AddInfrastructureRepositories();
            services.AddInfrastructureMessaging(configuration);
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
            services.AddScoped<IStockItemRepository, StockItemRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();

            return services;
        }

        private static IServiceCollection AddInfrastructureMessaging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection(KafkaOptions.SectionName))
                .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "Kafka:BootstrapServers must be provided.")
                .Validate(options => !string.IsNullOrEmpty(options.GroupId), "Kafka:GroupId must be provided.")
                .Validate(options => options.Topics.Length > 0, "At least one Kafka topic must be configured.")
                .Validate(options => !string.IsNullOrEmpty(options.WarehouseStockTopic), "Kafka:WarehouseStockTopic must be provided.")
                .Validate(options => options.Acks == "All" || options.Acks == "Leader" || options.Acks == "None", "Kafka:Acks must be 'All', 'Leader', or 'None'.")
                .ValidateOnStart();

            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            services.AddScoped<ProductLifecycleProcessor>();
            services.AddHostedService<KafkaConsumer>();
            services.AddHostedService<OutboxPublisher>();

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
