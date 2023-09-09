using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models;

namespace Redemption.Balancer.Api.Application.Common.Factories;

public class ResponseFactory : IResponseFactory
{
    public ApiResponse CreateApiResponse(object objectResultValue, string traceId)
    {
        if (objectResultValue is ErrorInfo errorInfo)
        {
            return new ApiResponse(errorInfo)
            {
                TraceId = traceId
            };
        }

        return new ApiResponse(objectResultValue)
        {
            TraceId = traceId
        };
    }
}