namespace Redemption.Balancer.Api.Application.Tracing;

public static class RequestTracingExtensions
{
    public static string GetRequestTraceId(this IHeaderDictionary headers)
    {
        headers.TryGetValue("trace-id", out var traceId);

        return traceId.ToString();
    }
}