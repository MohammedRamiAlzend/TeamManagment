using System.Text.Json;
using TMS.Core.Entities;
using TMS.Infrastructure.AppConfigurations;

namespace TMS.Infrastructure.AppConfigurations;



public static class PermissionSettings
{
    public static List<RoleModel> Roles { get; private set; }
    public static List<PermissionModel> Permissions { get; private set; }
    public static void LoadPermissionsConfig()
    {
        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "permissions.json");
        if (File.Exists(configPath))
        {
            try
            {
                var json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<PermissionsConfig>(json);
                if (config == null) return;
                
                Roles = config.Roles ?? [];
                Permissions = config.Permissions ?? [];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        else
        {
            throw new FileNotFoundException("Permissions configuration file not found.");
        }
    }
}



public class PermissionModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public static implicit operator Permission(PermissionModel permission) => new()
    {
        Name = permission.Name,
        Description = permission.Description
    };
    public static implicit operator PermissionModel(Permission permission) => new()
    {
        Name = permission.Name,
        Description = permission.Description
    };
}

public sealed class RoleModel
{
    public string Name { get; set; }
    public List<PermissionModel> Permissions { get; set; }
    public static implicit operator Role(RoleModel role)
    {
        var permissions = role.Permissions.Select(p => (Permission)p).ToList();
        return new Role
        {
            Name = role.Name,
            Permissions = permissions
        };
    }
}

public class PermissionsConfig
{
    public List<RoleModel> Roles { get; set; }
    public  List<PermissionModel> Permissions { get; set; }
}
