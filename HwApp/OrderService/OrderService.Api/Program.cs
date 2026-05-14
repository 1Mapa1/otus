using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OrderService.Api.Authentication;
using OrderService.Application;
using OrderService.Infrastructure;

namespace OrderService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });

                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    new List<string>());
                options.AddSecurityRequirement(securityRequirement);
            });

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddInfrastructureHealthChecks();

            builder.Services.AddHttpClient(JwksSigningKeyCache.HttpClientName, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(15);
            });
            builder.Services.AddSingleton<JwksSigningKeyCache>();
            builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "api/orders/swagger/{documentName}/swagger.json";
                });

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/api/orders/swagger/v1/swagger.json", "Order API V1");
                    c.RoutePrefix = "api/orders/swagger";
                });
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
            });

            app.MapHealthChecks("/health/startup", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("startup"),
            });

            app.MapControllers();
            app.Run();
        }
    }
}
