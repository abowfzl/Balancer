using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models;
using Redemption.Balancer.Api.Application.Tracing;
using System.Text;

namespace Redemption.Balancer.Api.Application.Wrappers;

public class ExceptionFilter : IAsyncExceptionFilter
{
    private readonly IWebHostEnvironment _env;
    private readonly IResponseFactory _responseFactory;

    public ExceptionFilter(IWebHostEnvironment env,
        IResponseFactory responseFactory)
    {
        _env = env;
        _responseFactory = responseFactory;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        var errorInfo = new ErrorInfo(context.Exception.Message);

        if (_env.IsProduction() is false)
        {
            var detailBuilder = new StringBuilder();
            CreateDetailsFromException(context.Exception, detailBuilder);

            errorInfo.Details = detailBuilder.ToString();
        }

        context.HttpContext.Response.StatusCode = GetStatusCode(context.Exception);

        var apiResponse = _responseFactory.CreateApiResponse(errorInfo, context.HttpContext.Response.Headers.GetRequestTraceId());

        context.Result = new ObjectResult(apiResponse);

        context.Exception = null!;
        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }

    private static void CreateDetailsFromException(Exception exception, StringBuilder detailBuilder)
    {
        detailBuilder.AppendLine($"EXCEPTION DETAIL:{exception.GetType().Name}:{exception.Message}");

        if (string.IsNullOrEmpty(exception.StackTrace) is false)
            detailBuilder.AppendLine($"STACK TRACE: {exception.StackTrace}");

        if (exception.InnerException is not null) CreateDetailsFromException(exception.InnerException, detailBuilder);
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException => StatusCodes.Status404NotFound,
            ForbiddenException => StatusCodes.Status403Forbidden,
            BadRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}