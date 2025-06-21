using System.Reflection;

namespace TMS.Contract;

public static class AppRoles
{
    public static readonly RoleDefinition Admin = new()
    {
        Name = "Admin",
        Permissions = AppPermissions.GetAllPermissions().ToList()
    };

    public static readonly RoleDefinition Manager = new()
    {
        Name = "Manager",
        Permissions =
        [
            AppPermissions.UserManagement.Get, AppPermissions.UserManagement.Register, AppPermissions.UserManagement.Update, AppPermissions.UserManagement.Delete,AppPermissions.UserManagement.Rami,
            AppPermissions.RoleManagement.Get, AppPermissions.RoleManagement.Update,
            AppPermissions.ProjectManagement.Get, AppPermissions.ProjectManagement.Add, AppPermissions.ProjectManagement.Update, AppPermissions.ProjectManagement.Delete,
            AppPermissions.TaskManagement.Get, AppPermissions.TaskManagement.Get, AppPermissions.TaskManagement.Update, AppPermissions.TaskManagement.Delete
        ]
    };
    
    public static readonly RoleDefinition TeamLeader = new()
    {
        Name = "TeamLeader",
        Permissions =
        [
            AppPermissions.UserManagement.Get, AppPermissions.UserManagement.Register, AppPermissions.UserManagement.Update,
            AppPermissions.ProjectManagement.Get, AppPermissions.ProjectManagement.Add, AppPermissions.ProjectManagement.Update,
            AppPermissions.TaskManagement.Get, AppPermissions.TaskManagement.Get, AppPermissions.TaskManagement.Update, AppPermissions.TaskManagement.Delete
        ]
    };

    public static readonly RoleDefinition Employee = new()
    {
        Name = "Employee",
        Permissions =
        [
            AppPermissions.UserManagement.Get,
            AppPermissions.ProjectManagement.Get,
            AppPermissions.TaskManagement.Get, AppPermissions.TaskManagement.Update,
            AppPermissions.UserManagement.Delete
        ]
    };
    public static readonly RoleDefinition Observer = new()
    {
        Name = "Observer",
        Permissions =
        [
            AppPermissions.UserManagement.Get,
            AppPermissions.ProjectManagement.Get,
            AppPermissions.TaskManagement.Get, AppPermissions.TaskManagement.Update,
            AppPermissions.UserManagement.Delete
        ]
    };
    
    public static IEnumerable<RoleDefinition> GetAllRoles()
    {
        return typeof(AppRoles)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(RoleDefinition))
            .Select(field => (RoleDefinition)field.GetValue(null));
    }
}
public class RoleDefinition
{
    public string Name { get; set; }
    public List<string> Permissions { get; set; } = new();
}
