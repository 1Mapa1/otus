using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CustomerService.Infrastructure.Ef;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString(DatabaseContext.CONNECTION_NAME)));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

await db.Database.MigrateAsync();
