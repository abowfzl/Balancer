using Redemption.Balancer.Api.Application.Common.Models;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IResponseFactory
{
    ApiResponse CreateApiResponse(object objectResultValue, string traceId);
}