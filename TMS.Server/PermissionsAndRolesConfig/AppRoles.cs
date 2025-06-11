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
            UserManagement.GetUser, UserManagement.AddUser, UserManagement.UpdateUser, UserManagement.DeleteUser,
            RoleManagement.GetRole, RoleManagement.UpdateRole,
            ProjectManagement.GetProject, ProjectManagement.AddProject, ProjectManagement.UpdateProject, ProjectManagement.DeleteProject,
            TaskManagement.GetTask, TaskManagement.AddTask, TaskManagement.UpdateTask, TaskManagement.DeleteTask
        }
    };
    
    public static readonly RoleDefinition TeamLeader = new()
    {
        Name = "TeamLeader",
        Permissions = new List<string>
        {
            UserManagement.GetUser, UserManagement.AddUser, UserManagement.UpdateUser,
            ProjectManagement.GetProject, ProjectManagement.AddProject, ProjectManagement.UpdateProject,
            TaskManagement.GetTask, TaskManagement.AddTask, TaskManagement.UpdateTask, TaskManagement.DeleteTask
        }
    };

    public static readonly RoleDefinition Employee = new()
    {
        Name = "Employee",
        Permissions = new List<string>
        {
            UserManagement.GetUser,
            ProjectManagement.GetProject,
            TaskManagement.GetTask, TaskManagement.UpdateTask,
            UserManagement.DeleteUser
        }
    };
    public static readonly RoleDefinition Observer = new()
    {
        Name = "Observer",
        Permissions = new List<string>
        {
            UserManagement.GetUser,
            ProjectManagement.GetProject,
            TaskManagement.GetTask, TaskManagement.UpdateTask,
            UserManagement.DeleteUser
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
