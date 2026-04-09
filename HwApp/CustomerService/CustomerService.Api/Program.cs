using CustomerService.Api.Mapping;
using CustomerService.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        []
                    }
                });
            });

            builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services
                .AddInfrastructureHealthChecks()
                .ForwardToPrometheus();

            builder.Services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = builder.Configuration["Auth:Url"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Auth:Url"],

                        ValidateAudience = false,

                        ValidateLifetime = true,

                        NameClaimType = "sub",
                    };
                });

            var app = builder.Build();

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

            app.UseAuthentication();
            app.UseAuthorization();

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
    }
}
