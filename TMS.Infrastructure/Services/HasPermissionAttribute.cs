using Microsoft.AspNetCore.Authorization;

namespace TMS.Infrastructure.Services;

public class HasPermissionAttribute : AuthorizeAttribute
{   
    public const string PolicyPrefix = "PERMISSIONS_";
    public const string Separator = ",";
    public HasPermissionAttribute(LogicalOperator op, params string[] permissions)
    {
        Policy = $"{PolicyPrefix}{op}:{string.Join(Separator, permissions)}";
    }

    public HasPermissionAttribute(params string[] permissions)
        : this(LogicalOperator.Or, permissions)
    {
    }

}

public enum LogicalOperator
{
    And,
    Or
}
