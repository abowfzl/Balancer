using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Exceptions;

namespace Redemption.Balancer.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[Route("v{version:apiVersion}/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected int GetUserIdFromHeader()
    {
        if (Request.Headers.TryGetValue(Constants.Application.Headers.XUserId, out var userIdHeaderValue) &&
            int.TryParse(userIdHeaderValue, out var userId)) return userId;

        throw new UnauthorizedException("User Not Authorized");
    }
}