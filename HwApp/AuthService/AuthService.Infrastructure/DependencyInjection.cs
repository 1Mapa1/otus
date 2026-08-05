using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Clients.CustomerService;
using AuthService.Infrastructure.Messaging.Kafka;
using AuthService.Infrastructure.Options;
using AuthService.Infrastructure.Rersistence;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Persistence.Outbox;
using AuthService.Infrastructure.Rersistence.Repositories;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Workers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IHttpClientBuilder>? configureHttpClient = null)
        {
            services.AddInfrastructureDatabaseContext(configuration);

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddInfrastructureOptions(configuration);

            services.AddInfrastructureHttpClients(configureHttpClient);

            if (configuration.GetValue("OutboxPublisherEnabled", true))
            {
                services.AddOptions<KafkaOptions>()
                    .Bind(configuration.GetSection(KafkaOptions.SectionName))
                    .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "BootstrapServers must be provided.")
                    .Validate(options => !string.IsNullOrEmpty(options.Acks), "Acks must be provided.")
                    .Validate(options => options.Acks == "All" || options.Acks == "Leader" || options.Acks == "None", "Acks must be 'All', 'Leader', or 'None'.")
                    .ValidateOnStart();

                services.AddSingleton<IKafkaProducer, KafkaProducer>();
                services.AddHostedService<OutboxPublisher>();
            }

            services.AddSingleton<RsaJwtSigningKeyProvider>();
            services.AddSingleton<IJwksProvider, JwksProvider>();
            services.AddSingleton<IPasswordHasherService, PasswordHashService>();
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }

        public static IHealthChecksBuilder AddInfrastructureHealthChecks(
            this IServiceCollection services,
            string healthCheckName = "Database", IEnumerable<string>? healthCheckTags = null)
        {
            healthCheckTags ??= ["ready", "startup"];

            return services.AddHealthChecks()
                .AddDbContextCheck<AuthDbContext>(
                    name: healthCheckName,
                    tags: healthCheckTags);
        }

        public static IServiceCollection AddInfrastructureDatabaseContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IIntegrationEventMapping, IntegrationEventMapping>();

            var connectionString = configuration.GetConnectionStringLocal();

            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        private static string GetConnectionStringLocal(
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
                throw new InvalidOperationException($"Connection string env or appsetings.\"Npgsql\" was not found.");

            return connectionString;
        }

        private static IServiceCollection AddInfrastructureHttpClients(
            this IServiceCollection services,
            Action<IHttpClientBuilder>? configureHttpClient)
        {
            var clientBuilder = services
                .AddHttpClient<ICustomerServiceClient, CustomerServiceClient>((sp, httpClient) =>
                {
                    var options = sp
                        .GetRequiredService<IOptions<CustomerServiceOptions>>()
                        .Value;

                    httpClient.BaseAddress = new Uri(options.BaseUrl);
                    httpClient.Timeout = options.Timeout;
                });

            configureHttpClient?.Invoke(clientBuilder);

            return services;
        }

        private static IServiceCollection AddInfrastructureOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<CustomerServiceOptions>()
                .Bind(configuration.GetSection(CustomerServiceOptions.SectionName))
                .Validate(
                    o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                    $"{CustomerServiceOptions.SectionName}:BaseUrl must be a valid absolute URI")
                .ValidateOnStart();

            services
             .AddOptions<JwtOptions>()
             .Bind(configuration.GetSection(JwtOptions.SectionName))
             .ValidateOnStart();

            return services;
        }

        public static async Task MigrationAsync(
            this IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            await db.Database.MigrateAsync();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            await SeedAdminAsync(db, configuration);
        }

        private static async Task SeedAdminAsync(
            AuthDbContext db,
            IConfiguration configuration)
        {
            var login = configuration["ADMIN_LOGIN"];
            var password = configuration["ADMIN_PASSWORD"];

            if (string.IsNullOrWhiteSpace(login) && string.IsNullOrWhiteSpace(password))
                return;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Both ADMIN_LOGIN and ADMIN_PASSWORD must be configured for the admin seed.");
            }

            var existingUser = await db.Users
                .SingleOrDefaultAsync(user => user.Login == login);

            if (existingUser is not null)
            {
                if (existingUser.Role != UserRole.Admin)
                {
                    throw new InvalidOperationException(
                        $"User '{login}' already exists but does not have the Admin role.");
                }

                var existingPasswordHasher = new PasswordHasher<User>();
                existingUser.UpdatePasswordHash(
                    existingPasswordHasher.HashPassword(existingUser, password));

                if (existingUser.Status != UserStatus.Active)
                    existingUser.Activate();

                await db.SaveChangesAsync();
                return;
            }

            var passwordHasher = new PasswordHasher<User>();
            var admin = new User(
                login,
                passwordHasher.HashPassword(null!, password),
                UserRole.Admin);

            admin.Activate();

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}
