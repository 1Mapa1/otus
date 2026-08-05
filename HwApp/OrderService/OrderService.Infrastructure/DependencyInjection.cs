using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderService.Application.Abstractions.Clients.Billing;
using OrderService.Application.Abstractions.Clients.Catalog;
using OrderService.Application.Abstractions.Clients.Delivery;
using OrderService.Application.Abstractions.Clients.Warehouse;
using OrderService.Application.Abstractions.Idempotency;
using OrderService.Application.Abstractions.Persistence;
using OrderService.Infrastructure.Clients.Billing;
using OrderService.Infrastructure.Clients.Catalog;
using OrderService.Infrastructure.Clients.Delivery;
using OrderService.Infrastructure.Clients.Warehouse;
using OrderService.Infrastructure.Idempotency;
using OrderService.Infrastructure.Messaging.Kafka;
using OrderService.Infrastructure.Options;
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
            IConfiguration configuration,
            Action<IHttpClientBuilder>? configureHttpClient = null)
        {
            services.AddInfrastructureDatabaseContext(configuration);

            services.AddInfrastructureClients(configuration, configureHttpClient);
            services.AddInfrastructureSaga(configuration);

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection(KafkaOptions.SectionName))
                .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "BootstrapServers must be provided.")
                .Validate(options => !string.IsNullOrEmpty(options.Acks), "Acks must be provided.")
                .Validate(options => options.Acks == "All" || options.Acks == "Leader" || options.Acks == "None", "Acks must be 'All', 'Leader', or 'None'.")
                .ValidateOnStart();

            services.AddSingleton<IKafkaProducer, KafkaProducer>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
            services.AddScoped<IRequestHashCalculator, RequestHashCalculator>();

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

        private static IServiceCollection AddInfrastructureSaga(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<OrderSagaOptions>()
                .Bind(configuration.GetSection(OrderSagaOptions.SectionName))
                .ValidateOnStart();

            services.AddHostedService<OrderSagaWorker>();

            return services;
        }

        private static IServiceCollection AddInfrastructureClients(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IHttpClientBuilder>? configureHttpClient)
        {
            services
                .AddOptions<BillingOptions>()
                .Bind(configuration.GetSection(BillingOptions.SectionName))
                .Validate(
                    o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                    $"{BillingOptions.SectionName}:BaseUrl must be a valid absolute URI")
                .ValidateOnStart();

            var billingBuilder = services.AddHttpClient<IBillingClient, BillingClient>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<BillingOptions>>().Value;
                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.Timeout = options.Timeout;
            });
            configureHttpClient?.Invoke(billingBuilder);

            services
               .AddOptions<WarehouseOptions>()
               .Bind(configuration.GetSection(WarehouseOptions.SectionName))
               .Validate(
                   o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                   $"{WarehouseOptions.SectionName}:BaseUrl must be a valid absolute URI")
               .ValidateOnStart();

            var warehouseBuilder = services.AddHttpClient<IWarehouseClient, WarehouseClient>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<WarehouseOptions>>().Value;
                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.Timeout = options.Timeout;
            });
            configureHttpClient?.Invoke(warehouseBuilder);

            services
                .AddOptions<CatalogOptions>()
                .Bind(configuration.GetSection(CatalogOptions.SectionName))
                .Validate(
                    o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                    $"{CatalogOptions.SectionName}:BaseUrl must be a valid absolute URI")
                .ValidateOnStart();

            var catalogBuilder = services.AddHttpClient<ICatalogClient, CatalogClient>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<CatalogOptions>>().Value;
                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.Timeout = options.Timeout;
            });
            configureHttpClient?.Invoke(catalogBuilder);

            services
                .AddOptions<DeliveryOptions>()
                .Bind(configuration.GetSection(DeliveryOptions.SectionName))
                .Validate(
                    o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                    $"{DeliveryOptions.SectionName}:BaseUrl must be a valid absolute URI")
                .ValidateOnStart();

            var deliveryBuilder = services.AddHttpClient<IDeliveryClient, DeliveryClient>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<DeliveryOptions>>().Value;
                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.Timeout = options.Timeout;
            });
            configureHttpClient?.Invoke(deliveryBuilder);

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
