using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderService.Application.Abstractions;
using OrderService.Application.Billing;
using OrderService.Application.Orders;
using OrderService.Domain.Orders;
using OrderService.Infrastructure.Billing;
using OrderService.Infrastructure.Clients.Billing;
using OrderService.Infrastructure.Messaging.Kafka;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.Outbox;
using OrderService.Infrastructure.Persistence.Repositories;
using OrderService.Infrastructure.Workers;

namespace OrderService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddInfrastructureDatabaseContext(configuration);

            services.AddInfrastructureBillingServiceClient(configuration);

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection(KafkaOptions.SectionName))
                .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "BootstrapServers must be provided.")
                .Validate(options => !string.IsNullOrEmpty(options.Acks), "Acks must be provided.")
                .Validate(options => options.Acks == "All" || options.Acks == "Leader" || options.Acks == "None", "Acks must be 'All', 'Leader', or 'None'.")
                .ValidateOnStart();

            services.AddSingleton<IKafkaProducer, KafkaProducer>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddHostedService<OutboxPublisher>();

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
            services.AddSingleton<IIntegrationEventMapping, IntegrationEventMapping>();

            var connectionString = configuration.GetConnectionStringLocal();

            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        public static IServiceCollection AddInfrastructureBillingServiceClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<BillingServiceOptions>()
                .Bind(configuration.GetSection(BillingServiceOptions.SectionName))
                .Validate(
                    o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                    $"{BillingServiceOptions.SectionName}:BaseUrl must be a valid absolute URI")
                .ValidateOnStart();

            services.AddHttpClient<IBillingServiceClient, BillingServiceClient>((sp, httpClient) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<BillingServiceOptions>>()
                    .Value;

                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.Timeout = options.Timeout;
            });

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
            {
                throw new InvalidOperationException(
                    "Connection string env or appsettings Npgsql was not found.");
            }

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
