using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddInfrastructureHealthChecks();

            var authUrl = builder.Configuration["Auth:Url"]!.TrimEnd('/');
            var jwksUrl = $"{authUrl}/.well-known/jwks.json";

            using var httpClient = new HttpClient();
            var jwksJson = httpClient.GetStringAsync(jwksUrl).GetAwaiter().GetResult();
            var jwks = new JsonWebKeySet(jwksJson);

            Console.WriteLine($"AUTH URL: {authUrl}");
            Console.WriteLine($"JWKS URL: {jwksUrl}");
            Console.WriteLine($"JWKS keys count: {jwks.Keys.Count}");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authUrl,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = jwks.GetSigningKeys(),
                        NameClaimType = "sub",
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"AUTH FAILED: {context.Exception}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("TOKEN VALIDATED");
                            Console.WriteLine($"sub = {context.Principal?.Identity?.Name}");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine($"AUTH CHALLENGE: {context.Error}; {context.ErrorDescription}");
                            return Task.CompletedTask;
                        }
                    };
                });

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
            app.Run();
        }
    }
}
