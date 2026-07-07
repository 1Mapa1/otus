using AuthService.Api.Observability;
using AuthService.Application;
using AuthService.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using Prometheus.HttpMetrics;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, _, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(new RenderedCompactJsonFormatter()));

    builder.Services.AddSerilog((_, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(new RenderedCompactJsonFormatter()));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<RequestIdPropagationHandler>();

    builder.Services
        .AddApplication()
        .AddInfrastructure(
            builder.Configuration,
            clientBuilder => clientBuilder.AddHttpMessageHandler<RequestIdPropagationHandler>());
    builder.Services
        .AddInfrastructureHealthChecks()
        .ForwardToPrometheus();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/auth/swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/auth/swagger/v1/swagger.json", "Auth API V1");
            c.RoutePrefix = "api/auth/swagger";
        });
    }

    app.UseRouting();
    app.UseMiddleware<RequestIdMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = static (httpContext, _, ex) =>
        {
            if (ex is not null)
            {
                return LogEventLevel.Error;
            }

            var path = httpContext.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/metrics", StringComparison.OrdinalIgnoreCase))
            {
                return LogEventLevel.Debug;
            }

            return httpContext.Response.StatusCode switch
            {
                >= 500 => LogEventLevel.Error,
                >= 400 => LogEventLevel.Warning,
                _ => LogEventLevel.Information,
            };
        };
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
                    0.001, 0.0025, 0.005, 0.0075, 0.01, 0.025, 0.05, 0.1,
                    0.25, 0.5, 1.0, 2.5, 5.0, 10.0, 30.0,
                ],
            });
    });

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

    app.MapControllers();

    app.MapMetrics();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
