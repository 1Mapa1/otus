using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Clients.BillingService;
using AuthService.Infrastructure.Clients.CustomerService;
using AuthService.Infrastructure.Rersistence;
using AuthService.Infrastructure.Rersistence.Repositories;
using AuthService.Infrastructure.Security;
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
            IConfiguration configuration)
        {
            services.AddInfrastructureDatabaseContext(configuration);

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddInfrastructureOptions(configuration);

            services.AddInfrastructureHttpClients();

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
            this IServiceCollection services)
        {
            services.AddHttpClient<ICustomerServiceClient, CustomerServiceClient>((sp, httpClient) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<CustomerServiceOptions>>()
                    .Value;

                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.Timeout = options.Timeout;
            });

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
                .AddOptions<BillingServiceOptions>()
                .Bind(configuration.GetSection(BillingServiceOptions.SectionName))
                .Validate(
                    o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                    $"{BillingServiceOptions.SectionName}:BaseUrl must be a valid absolute URI")
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
        }
    }
}
