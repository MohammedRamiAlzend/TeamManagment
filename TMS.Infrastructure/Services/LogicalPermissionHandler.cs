using Microsoft.AspNetCore.Authorization;

namespace TMS.Infrastructure.Services;

public class LogicalPermissionHandler : AuthorizationHandler<LogicalPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        LogicalPermissionRequirement requirement)
    {
        var userPermissions = context.User.FindAll("permission").Select(c => c.Value);

        if (requirement.Operator == LogicalOperator.And)
        {
            // If ALL required permissions are present in the user's claims, succeed.
            if (requirement.Permissions.All(p => userPermissions.Contains(p)))
            {
                context.Succeed(requirement);
            }
        }
        else // LogicalOperator.Or
        {
            // If ANY of the required permissions are present, succeed.
            if (requirement.Permissions.Any(p => userPermissions.Contains(p)))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

