namespace Redemption.Balancer.Api.Application.Tracing;

public class RequestTracingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTracingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestTraceId = context.Request.Headers.GetRequestTraceId();

        string traceId;
        if (!string.IsNullOrEmpty(requestTraceId))
            traceId = requestTraceId;
        else
            traceId = Guid.NewGuid().ToString();

        context.Response.Headers.TryAdd("trace-id", traceId);

        await _next(context);
    }
}