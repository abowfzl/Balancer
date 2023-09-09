namespace Redemption.Balancer.Api.Application.Common.Models;

[Serializable]
public class ApiResponse<TResult> : ApiResponseBase
{
    public ApiResponse(TResult result)
    {
        Success = true;
        Result = result;
    }

    public ApiResponse(ErrorInfo error)
    {
        Success = false;
        Error = error;
    }

    public TResult? Result { get; set; }
}

[Serializable]
public class ApiResponse : ApiResponse<object>
{
    public ApiResponse(object result) : base(result)
    {
    }

    public ApiResponse(ErrorInfo error) : base(error)
    {
    }
}