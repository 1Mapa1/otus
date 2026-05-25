using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Customers;
using NotificationService.Domain.Notifications;
using NotificationService.Infrastructure.Messaging.Kafka;
using NotificationService.Infrastructure.Messaging.Kafka.HealthCheck;
using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Persistence.Repositories;
using NotificationService.Infrastructure.Workers;

namespace NotificationService.Infrastructure
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

            services.AddInfrastructureKafka(configuration);

            return services;
        }

        public static IHealthChecksBuilder AddInfrastructureHealthChecks(
            this IServiceCollection services,
            IEnumerable<string>? healthCheckTags = null)
        {
            healthCheckTags ??= ["ready", "startup"];

            return services.AddHealthChecks()
                .AddDbContextCheck<DatabaseContext>(
                    name: "Database",
                    tags: healthCheckTags)
                .AddCheck<KafkaConsumerHealthCheck>(
                    name: "KafkaConsumer",
                    failureStatus: HealthStatus.Unhealthy,
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

        private static IServiceCollection AddInfrastructureRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<INotificationCustomerRepository, NotificationCustomerRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }

        private static IServiceCollection AddInfrastructureKafka(
            this IServiceCollection services,
            IConfiguration configuration)   
        {
            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection(KafkaOptions.SectionName))
                .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "BootstrapServers must be provided.")
                .Validate(options => !string.IsNullOrEmpty(options.GroupId), "GroupId must be provided.")
                .Validate(options => options.Topics.Length > 0)
                .ValidateOnStart();

            services.AddSingleton<KafkaConsumerState>();

            services.AddScoped<KafkaMessageDispatcher>();

            services.AddScoped<IKafkaIntegrationEventHandler, CustomerCreatedEventHandler>();
            services.AddScoped<IKafkaIntegrationEventHandler, CustomerUpdatedEventHandler>();
            services.AddScoped<IKafkaIntegrationEventHandler, OrderPaidEventHandler>();
            services.AddScoped<IKafkaIntegrationEventHandler, OrderRejectedEventHandler>();

            services.AddHostedService<KafkaConsumer>();

            return services;
        }

        internal static string GetConnectionStringLocal(
            this IConfiguration configuration)
        {
            var connectionString = !string.IsNullOrEmpty(configuration["DB_HOST"])
            ? $"Host={configuration["DB_HOST"]};" +
              $"Port={configuration["DB_PORT"]};" +
              $"Database={configuration["DB_NAME"]};" +
              $"Username={configuration["DB_USER"]};" +
              $"Password={configuration["DB_PASSWORD"]}"
            : configuration.GetConnectionString("Npgsql");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string env or appsetings.Npgsql was not found.");

            return connectionString;
        }

        public static async Task MigrationAsync(
            this IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            await db.Database.MigrateAsync();
        }
    }
}
