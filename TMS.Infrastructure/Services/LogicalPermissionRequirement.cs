namespace TMS.Infrastructure.Services;

public class LogicalPermissionRequirement(LogicalOperator op, string[] permissions)
    : IAuthorizationRequirement
{
    public LogicalOperator Operator { get; } = op;
    public string[] Permissions { get; } = permissions;
}