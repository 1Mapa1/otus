using CustomerService.Domain.Interfaces;
using CustomerService.Infrastructure.Messaging.Kafka;
using CustomerService.Infrastructure.Persistence;
using CustomerService.Infrastructure.Persistence.Outbox;
using CustomerService.Infrastructure.Repositories;
using CustomerService.Infrastructure.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CustomerService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddInfrastructureDatabaseContext(configuration);

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection(KafkaOptions.SectionName))
                .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "BootstrapServers must be provided.")
                .Validate(options => !string.IsNullOrEmpty(options.Acks), "Acks must be provided.")
                .Validate(options => options.Acks == "All" || options.Acks == "Leader" || options.Acks == "None", "Acks must be 'All', 'Leader', or 'None'.")
                .ValidateOnStart();

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddSingleton<IIntegrationEventMapping, IntegrationEventMapping>();

            services.AddSingleton<IKafkaProducer, KafkaProducer>();

            services.AddHostedService<OutboxPublisher>();

            return services;
        }

        public static IHealthChecksBuilder AddInfrastructureHealthChecks(
            this IServiceCollection services,
            string healthCheckName = "Database", IEnumerable<string>? healthCheckTags = null)
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

        internal static string GetConnectionStringLocal(
            this IConfiguration configuration)
        {
            var connectionString = !string.IsNullOrEmpty(configuration["DB_HOST"])
            ? $"Host={configuration["DB_HOST"]};" +
              $"Port={configuration["DB_PORT"]};" +
              $"Database={configuration["DB_NAME"]};" +
              $"Username={configuration["DB_USER"]};" +
              $"Password={configuration["DB_PASSWORD"]}"
            : configuration.GetConnectionString(DatabaseContext.CONNECTION_NAME);

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"Connection string env or appsetings.{DatabaseContext.CONNECTION_NAME} was not found.");

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
