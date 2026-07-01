using CatalogService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
        services.AddInfrastructureWriteDatabaseContext(context.Configuration))
    .Build();

await host.Services.MigrationAsync();
await host.Services.SeedCatalogAsync();
