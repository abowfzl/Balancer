using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models;
using Redemption.Balancer.Api.Application.Tracing;
using System.Text;

namespace Redemption.Balancer.Api.Application.Wrappers;

public class ResultFilter : IAsyncResultFilter
{
    private readonly IResponseFactory _responseFactory;

    public ResultFilter(IResponseFactory responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is not ObjectResult objectResult)
            return;

        var traceId = context.HttpContext.Response.Headers.GetRequestTraceId();

        var objectResultValue = objectResult.Value;

        if (objectResultValue == null)
        {
            await next();
            return;
        }

        var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

        if (statusCode != StatusCodes.Status200OK)
        {
            objectResultValue = new ErrorInfo(CreateErrorInfoMessage(objectResultValue));
        }

        var apiResponse = _responseFactory.CreateApiResponse(objectResultValue, traceId);

        context.Result = new ObjectResult(apiResponse)
        {
            StatusCode = statusCode
        };

        await next();
    }

    private static string CreateErrorInfoMessage(object response)
    {
        var stringBuilder = new StringBuilder();

        switch (response)
        {
            case ValidationProblemDetails validationErrors:
                foreach (var validationErrorValue in validationErrors.Errors.Values.SelectMany(value => value))
                {
                    stringBuilder.AppendLine(validationErrorValue);
                }
                break;

            default:
                stringBuilder.AppendLine("An error has occurred");
                break;
        }

        return stringBuilder.ToString();
    }
}