namespace Redemption.Balancer.Api.Application.Common.Models.Externals;

public class BaseResponse
{
    public bool Success { get; set; }

    public object Result { get; set; } = null!;

    public ErrorInfo Error { get; set; } = null!;

    public class ErrorInfo
    {
        public string Message { get; set; } = null!;

        public string? Details { get; set; }
    }
}