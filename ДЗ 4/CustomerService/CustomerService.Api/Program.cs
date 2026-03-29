using CustomerService.Api.Mapping;
using CustomerService.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;

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

            app.UseHttpMetrics(options =>
            {
                options.RequestDuration.Histogram = Metrics.CreateHistogram(
                    "http_request_duration_seconds", 
                    "Duration of HTTP requests in seconds",
                    new HistogramConfiguration
                    {
                        Buckets =
                        [
                            0.001,   // 1 ms
                            0.0025,  // 2.5 ms
                            0.005,   // 5 ms
                            0.0075,  // 7.5 ms
                            0.01,    // 10 ms
                            0.025,   // 25 ms
                            0.05,    // 50 ms
                            0.1,     // 100 ms
                            0.25,    // 250 ms
                            0.5,     // 500 ms
                            1.0      // 1 s
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
