namespace OrderService.Api.Observability;

internal sealed class RequestIdPropagationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestIdPropagationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Remove(RequestIdMiddleware.HeaderName);

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null &&
            httpContext.Request.Headers.TryGetValue(RequestIdMiddleware.HeaderName, out var requestId) &&
            !string.IsNullOrWhiteSpace(requestId))
        {
            request.Headers.TryAddWithoutValidation(
                RequestIdMiddleware.HeaderName,
                requestId.ToString());
        }

        return base.SendAsync(request, cancellationToken);
    }
}
