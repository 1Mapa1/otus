using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace WarehouseService.Api.Observability;

internal sealed class RequestIdMiddleware
{
    public const string HeaderName = "X-Request-ID";

    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(requestId))
        {
            requestId = Guid.NewGuid().ToString("D");
        }

        context.TraceIdentifier = requestId;
        context.Request.Headers[HeaderName] = requestId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = requestId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("RequestId", requestId))
        {
            await _next(context);
        }
    }
}

internal sealed class HttpRequestIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var requestId = _httpContextAccessor.HttpContext?
            .Request.Headers[RequestIdMiddleware.HeaderName]
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(requestId))
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("RequestId", requestId));
        }
    }
}
