namespace TMS.Infrastructure.Services;

public class LogicalPermissionHandler : AuthorizationHandler<LogicalPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        LogicalPermissionRequirement requirement)
    {
        var userPermissions = context.User.FindAll(AuthHelper.PermissionClaimName).Select(c => c.Value);

        if (requirement.Operator == LogicalOperator.And)
        {
            if (requirement.Permissions.All(p => userPermissions.Contains(p))) context.Succeed(requirement);
        }
        else
        {
            if (requirement.Permissions.Any(p => userPermissions.Contains(p))) context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}