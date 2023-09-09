using AspNetCore.Http.Extensions;
using Redemption.Balancer.Api.Application.Common.Exceptions;

namespace Redemption.Balancer.Api.Infrastructure.Clients;

public abstract class BaseClient
{
    protected async Task<T> GetResponse<T>(string clientSourceName, HttpResponseMessage httpResponseMessage)
    {
        return httpResponseMessage.StatusCode switch
        {
            System.Net.HttpStatusCode.OK => await httpResponseMessage.Content.ReadAsJsonAsync<T>(),
            System.Net.HttpStatusCode.NotFound => throw new EntityNotFoundException($"Entity not found from {clientSourceName}"),
            System.Net.HttpStatusCode.BadRequest => throw new BadRequestException($"Bad Request from {clientSourceName}"),
            System.Net.HttpStatusCode.Forbidden => throw new ForbiddenException($"Forbidden Request from {clientSourceName}"),
            System.Net.HttpStatusCode.Unauthorized => throw new UnauthorizedException($"Unauthorized Request from {clientSourceName}"),
            _ => throw new Exception(
                $"no response during call {clientSourceName}. status code: {httpResponseMessage.StatusCode}")
        };
    }
}