using CustomerService.Domain.Interfaces;
using CustomerService.Infrastructure.Ef;
using CustomerService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var conectionString = configuration.GetConnectionString(DatabaseContext.CONNECTION_NAME) 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found."); ;

            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(conectionString));

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            return services;
        }
    }
}
