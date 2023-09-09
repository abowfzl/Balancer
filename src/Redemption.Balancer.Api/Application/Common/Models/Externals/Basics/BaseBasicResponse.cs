namespace Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;

public class BaseBasicResponse
{
    public bool Success { get; set; }

    public object Result { get; set; }

    public ErrorInfo Error { get; set; }

    public class ErrorInfo
    {
        public string Message { get; set; }

        public string? Details { get; set; }
    }
}