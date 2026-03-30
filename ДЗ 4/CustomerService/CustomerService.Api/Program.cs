using CustomerService.Api.Mapping;
using CustomerService.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using Prometheus.HttpMetrics;

namespace CustomerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services
                .AddInfrastructureHealthChecks()
                .ForwardToPrometheus();

            var app = builder.Build();

            app.Use(async (context, next) =>
            {
                try
                {
                    await next(context);
                }
                catch
                {
                    context.Response.StatusCode = 500;
                    throw;
                }
            });


            app.UseHttpMetrics(options =>
            {
                options.RequestDuration.Histogram = Metrics.CreateHistogram(
                    "http_request_duration_seconds", 
                    "Duration of HTTP requests in seconds",
                    labelNames: HttpRequestLabelNames.All,
                    configuration: new HistogramConfiguration
                    {
                        Buckets =
                        [
                            0.001,   
                            0.0025, 
                            0.005,   
                            0.0075,  
                            0.01,    
                            0.025,   
                            0.05,    
                            0.1,     
                            0.25,    
                            0.5,     
                            1.0,
                            2.5,
                            5.0,
                            10.0,
                            30.0
                        ]
                    });
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            app.MapHealthChecks("/health/startup", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("startup")
            });

            app.MapMetrics();

            app.Run();
        }
    }
}
