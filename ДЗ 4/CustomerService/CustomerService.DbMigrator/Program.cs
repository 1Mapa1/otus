using CustomerService.Infrastructure;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructureDatabaseContext(context.Configuration);
    })
    .Build();

await host.Services.MigrationAsync();