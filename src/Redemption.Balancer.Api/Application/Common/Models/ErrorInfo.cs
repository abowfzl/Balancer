namespace Redemption.Balancer.Api.Application.Common.Models;

public class ErrorInfo
{
    public ErrorInfo(string message)
    {
        Message = message;
    }

    public string Message { get; set; }

    public string? Details { get; set; }
}