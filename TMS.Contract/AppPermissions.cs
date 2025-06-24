namespace TMS.Contract;

public static class AppPermissions
{
    public static class UserManagement
    {
        private const string GetPrefix = nameof(UserManagement);
        public const string Get = $"{GetPrefix}.Get";
        public const string Register = $"{GetPrefix}.Register";
        public const string Update = $"{GetPrefix}.Update";
        public const string Delete = $"{GetPrefix}.Delete";
    }
    public static class DepartmentManagement
    {
        private const string GetPrefix = nameof(DepartmentManagement);
        public const string Get = $"{GetPrefix}.Get";
        public const string Add = $"{GetPrefix}.Add";
        public const string Update = $"{GetPrefix}.Update";
        public const string Delete = $"{GetPrefix}.Delete";
    }
    public static class EmployeeManagement
    {
        private const string GetPrefix = nameof(EmployeeManagement);
        public const string Get = $"{GetPrefix}.Get";
        public const string Update = $"{GetPrefix}.Update";
        public const string Delete = $"{GetPrefix}.Delete";
    }
    public static class RoleManagement
    {
        private const string GetPrefix = nameof(RoleManagement);
        public const string Add = $"{GetPrefix}.Add";
        public const string Get = $"{GetPrefix}.Get";
        public const string Update = $"{GetPrefix}.Update";
    }

    public static class ProjectManagement
    {
        private const string GetPrefix = nameof(ProjectManagement);
        public const string Get = $"{GetPrefix}.Get";
        public const string Add = $"{GetPrefix}.Add";
        public const string Update = $"{GetPrefix}.Update";
        public const string Delete = $"{GetPrefix}.Delete";
    }

    public static class TaskManagement
    {
        private const string GetPrefix = nameof(TaskManagement);
        public const string Get = $"{GetPrefix}.Get";
        public const string Add = $"{GetPrefix}.Add";
        public const string Update = $"{GetPrefix}.Update";
        public const string Delete = $"{GetPrefix}.Delete";
        public const string SubmitTask = $"{GetPrefix}.Delete";
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
}


