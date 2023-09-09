using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RoleAttribute : Attribute
{
    public Role[] AllowedRoles { get; set; }

    public RoleAttribute(Role[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}