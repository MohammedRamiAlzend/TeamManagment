namespace TMS.Server.PermissionsAndRolesConfig;

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
        Permissions = new List<string>
        {
            UserManagement.Get, UserManagement.Register, UserManagement.Update, UserManagement.Delete,
            RoleManagement.Get, RoleManagement.Update,
            ProjectManagement.Get, ProjectManagement.Add, ProjectManagement.Update, ProjectManagement.Delete,
            TaskManagement.Get, TaskManagement.Get, TaskManagement.Update, TaskManagement.Delete
        }
    };
    
    public static readonly RoleDefinition TeamLeader = new()
    {
        Name = "TeamLeader",
        Permissions = new List<string>
        {
            UserManagement.Get, UserManagement.Register, UserManagement.Update,
            ProjectManagement.Get, ProjectManagement.Add, ProjectManagement.Update,
            TaskManagement.Get, TaskManagement.Get, TaskManagement.Update, TaskManagement.Delete
        }
    };

    public static readonly RoleDefinition Employee = new()
    {
        Name = "Employee",
        Permissions = new List<string>
        {
            UserManagement.Get,
            ProjectManagement.Get,
            TaskManagement.Get, TaskManagement.Update,
            UserManagement.Delete
        }
    };
    public static readonly RoleDefinition Observer = new()
    {
        Name = "Observer",
        Permissions = new List<string>
        {
            UserManagement.Get,
            ProjectManagement.Get,
            TaskManagement.Get, TaskManagement.Update,
            UserManagement.Delete
        }
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
