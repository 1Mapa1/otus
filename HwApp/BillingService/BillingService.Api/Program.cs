using BillingService.Api.Authentication;
using BillingService.Api.Observability;
using BillingService.Application;
using BillingService.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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
        .Enrich.With<HttpRequestIdEnricher>()
        .WriteTo.Console(new RenderedCompactJsonFormatter()));

    builder.Services.AddSerilog((_, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.With<HttpRequestIdEnricher>()
        .WriteTo.Console(new RenderedCompactJsonFormatter()));

    builder.Services.AddControllers();
    builder.Services.AddHttpLogging(options =>
    {
        options.LoggingFields =
            HttpLoggingFields.RequestProperties |
            HttpLoggingFields.RequestBody |
            HttpLoggingFields.ResponseStatusCode |
            HttpLoggingFields.ResponseBody;
        options.CombineLogs = true;
    });
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
    builder.Services.AddInfrastructureHealthChecks()
        .ForwardToPrometheus();

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
            c.RouteTemplate = "api/billing/swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/billing/swagger/v1/swagger.json", "Billing API V1");
            c.RoutePrefix = "api/billing/swagger";
        });
    }

    app.UseRouting();
    app.UseMiddleware<RequestIdMiddleware>();
    app.UseHttpLogging();
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
