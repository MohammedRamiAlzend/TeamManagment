using System.Text.Json.Serialization;

namespace TMS.Server.PermissionsAndRolesConfig;

public static class AppPermissions
{
    public static class UserManagement
    {
        public const string GetUser = "UserManagement.GetUser";
        public const string AddUser = "UserManagement.AddUser";
        public const string UpdateUser = "UserManagement.UpdateUser";
        public const string DeleteUser = "UserManagement.DeleteUser";
        public const string DeleteUser2 = "UserManagement.DeleteUser2";
    }
    public static class RoleManagement
    {
        public const string GetRole = "RoleManagement.GetRole";
        public const string UpdateRole = "RoleManagement.UpdateRole";
        public const string GetPermission = "RoleManagement.GetPermission";
        public const string AddPermission = "RoleManagement.AddPermission";
        public const string UpdatePermission = "RoleManagement.UpdatePermission";
        public const string RemovePermission = "RoleManagement.RemovePermission";
    }

    public static class ProjectManagement
    {
        public const string GetProject = "ProjectManagement.GetProject";
        public const string AddProject = "ProjectManagement.AddProject";
        public const string UpdateProject = "ProjectManagement.UpdateProject";
        public const string DeleteProject = "ProjectManagement.DeleteProject";
    }

    public static class TaskManagement
    {
        public const string GetTask = "TaskManagement.GetTask";
        public const string AddTask = "TaskManagement.AddTask";
        public const string UpdateTask = "TaskManagement.UpdateTask";
        public const string DeleteTask = "TaskManagement.DeleteTask";
    }
    public static IEnumerable<string> GetAllPermissions()
    {
        var permissionTypes = typeof(AppPermissions)
            .GetNestedTypes(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        foreach (var type in permissionTypes)
        {
            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    yield return (string)field.GetRawConstantValue();
                }
            }
        }
    }
}



public class AppDataModel
{
    [JsonPropertyName("Version")]
    public int Version { get; set; }

    [JsonPropertyName("Roles")]
    public List<RoleModel> Roles { get; set; } = new();

    [JsonPropertyName("Permissions")]
    public List<PermissionDetailModel> Permissions { get; set; } = new();
}
public class RoleModel
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Permissions")]
    public List<PermissionDetailModel> Permissions { get; set; } = new();
}

public class PermissionDetailModel
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }
}


