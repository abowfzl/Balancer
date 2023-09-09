using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Extensions;

namespace Redemption.Balancer.Api.Application.Wrappers;

public class RoleFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var endPoint = context.HttpContext.GetEndpoint();
        var roleAttribute = endPoint!.Metadata.GetMetadata<RoleAttribute>();

        if (roleAttribute != null)
        {
            if (context.HttpContext.Request.Headers.TryGetValue(Constants.Application.Headers.XUserRoles,
                    out var headerRoles) is false)
                throw new ForbiddenException("You don't have permission to access this resource!");

            var roleList = JsonConvert.DeserializeObject<List<string>>(headerRoles);

            if (roleList == null || roleList.Any() is false)
                throw new ForbiddenException("You don't have permission to access this resource!");

            if (roleList.Any(role => roleAttribute.AllowedRoles.Contains(role.ConvertToRoleEnum())) is false)
                throw new ForbiddenException("Access denied");
        }

        await next();
    }
}