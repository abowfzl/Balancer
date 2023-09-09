namespace Redemption.Balancer.Api.Application.Common.Models;

public abstract class ApiResponseBase
{
    public bool Success { get; set; }

    public ErrorInfo? Error { get; set; }

    public string? TraceId { get; set; }
}