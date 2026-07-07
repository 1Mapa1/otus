using Serilog.Context;

namespace CatalogService.Api.Observability;

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
