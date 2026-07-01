using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Infrastructure.Caching;
using CatalogService.Infrastructure.Messaging;
using CatalogService.Infrastructure.Messaging.Kafka;
using CatalogService.Infrastructure.Options;
using CatalogService.Infrastructure.Persistence;
using CatalogService.Infrastructure.Persistence.Outbox;
using CatalogService.Infrastructure.Persistence.Repositories;
using CatalogService.Infrastructure.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CatalogService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IIntegrationEventMapping, IntegrationEventMapping>();

            services.AddInfrastructureDatabaseContexts(configuration);
            services.AddInfrastructureRepositories();
            services.AddInfrastructureCaching(configuration);
            services.AddInfrastructureMessaging(configuration);
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IHealthChecksBuilder AddInfrastructureHealthChecks(this IServiceCollection services)
        {
            return services.AddHealthChecks()
                .AddDbContextCheck<CatalogWriteDbContext>(
                    name: "CatalogPrimary",
                    tags: ["ready", "startup"])
                .AddDbContextCheck<CatalogReadDbContext>(
                    name: "CatalogReplica",
                    tags: ["replica"]);
        }

        public static IServiceCollection AddInfrastructureWriteDatabaseContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetPrimaryConnectionString();

            services.AddDbContext<CatalogWriteDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        public static IServiceCollection AddInfrastructureDatabaseContexts(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var primaryConnectionString = configuration.GetPrimaryConnectionString();
            var replicaConnectionString = configuration.GetReplicaConnectionString();

            services.AddDbContext<CatalogWriteDbContext>(options =>
                options.UseNpgsql(primaryConnectionString));

            services.AddDbContext<CatalogReadDbContext>(options =>
                options.UseNpgsql(replicaConnectionString));

            return services;
        }

        private static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBrandWriteRepository, BrandWriteRepository>();
            services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IProductSnapshotRepository, ProductSnapshotRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IBrandReadRepository, BrandReadRepository>();
            services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();

            return services;
        }

        private static IServiceCollection AddInfrastructureCaching(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConfiguration = configuration["Redis:Configuration"];

            if (!string.IsNullOrWhiteSpace(redisConfiguration))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfiguration;
                    options.InstanceName = configuration["Redis:InstanceName"] ?? "catalog:";
                });

                services.AddSingleton(new RedisConnectionAccessor(ConnectionMultiplexer.Connect(redisConfiguration)));
            }
            else
            {
                services.AddDistributedMemoryCache();
                services.AddSingleton(new RedisConnectionAccessor(null));
            }

            services.AddScoped<IProductListCacheService, ProductListCacheService>();

            return services;
        }

        private static IServiceCollection AddInfrastructureMessaging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<CatalogOptions>()
                .Bind(configuration.GetSection(CatalogOptions.SectionName))
                .Validate(options => options.LowStockThreshold >= 0, "Catalog:LowStockThreshold must be zero or greater.")
                .ValidateOnStart();

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection(KafkaOptions.SectionName))
                .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "Kafka:BootstrapServers must be provided.")
                .Validate(options => !string.IsNullOrEmpty(options.GroupId), "Kafka:GroupId must be provided.")
                .Validate(options => options.Topics.Length > 0, "At least one Kafka topic must be configured.")
                .Validate(options => !string.IsNullOrEmpty(options.CatalogProductTopic), "Kafka:CatalogProductTopic must be provided.")
                .Validate(options => options.Acks == "All" || options.Acks == "Leader" || options.Acks == "None", "Kafka:Acks must be 'All', 'Leader', or 'None'.")
                .ValidateOnStart();

            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            services.AddScoped<StockChangedProcessor>();
            services.AddHostedService<KafkaConsumer>();
            services.AddHostedService<OutboxPublisher>();

            return services;
        }

        internal static string GetPrimaryConnectionString(this IConfiguration configuration)
        {
            var connectionString = !string.IsNullOrEmpty(configuration["DB_PRIMARY_HOST"])
                ? $"Host={configuration["DB_PRIMARY_HOST"]};" +
                  $"Port={configuration["DB_PRIMARY_PORT"]};" +
                  $"Database={configuration["DB_PRIMARY_NAME"]};" +
                  $"Username={configuration["DB_PRIMARY_USER"]};" +
                  $"Password={configuration["DB_PRIMARY_PASSWORD"]}"
                : configuration.GetConnectionString("CatalogPrimary");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Catalog primary connection string was not found.");

            return connectionString;
        }

        internal static string GetReplicaConnectionString(this IConfiguration configuration)
        {
            var connectionString = !string.IsNullOrEmpty(configuration["DB_REPLICA_HOST"])
                ? $"Host={configuration["DB_REPLICA_HOST"]};" +
                  $"Port={configuration["DB_REPLICA_PORT"]};" +
                  $"Database={configuration["DB_REPLICA_NAME"]};" +
                  $"Username={configuration["DB_REPLICA_USER"]};" +
                  $"Password={configuration["DB_REPLICA_PASSWORD"]}"
                : configuration.GetConnectionString("CatalogReplica");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Catalog replica connection string was not found.");

            return connectionString;
        }

        public static async Task MigrationAsync(this IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CatalogWriteDbContext>();

            await db.Database.MigrateAsync();
        }

        public static async Task SeedCatalogAsync(this IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CatalogWriteDbContext>();

            await CatalogDataSeeder.SeedAsync(db);
        }
    }
}
