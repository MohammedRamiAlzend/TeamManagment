namespace TMS.Server.PermissionsAndRolesConfig;

public static class DataSynchronizer
{
    public static void Synchronize(string jsonFilePath)
    {
        var permissionsFromCode = AppPermissions.GetAllPermissions().ToHashSet();
        var rolesFromCode = AppRoles.GetAllRoles().ToList();

        var appData = LoadAppData(jsonFilePath);

        var permissionsChanged = SynchronizePermissions(appData, permissionsFromCode);
        var rolesChanged = SynchronizeRoles(appData, rolesFromCode);

        if (permissionsChanged || rolesChanged)
        {
            SaveChanges(appData, jsonFilePath);
        }
    }

    private static void SaveChanges(AppDataModel appData, string jsonFilePath)
    {
        appData.Version++;

        var options = new JsonSerializerOptions { WriteIndented = true };
        var newJson = JsonSerializer.Serialize(appData, options);
        File.WriteAllText(jsonFilePath, newJson);
    }

    private static bool SynchronizeRoles(AppDataModel appData, IEnumerable<RoleDefinition> rolesFromCode)
    {
        var masterPermissionMap = appData.Permissions.ToDictionary(p => p.Name);
        var updatedRoles = new List<RoleModel>();

        foreach (var roleDef in rolesFromCode.OrderBy(r => r.Name))
        {
            var newRole = new RoleModel { Name = roleDef.Name };
            foreach (var permName in roleDef.Permissions.OrderBy(p => p))
            {
                if (masterPermissionMap.TryGetValue(permName, out var permissionDetail))
                {
                    newRole.Permissions.Add(permissionDetail);
                }
            }

            updatedRoles.Add(newRole);
        }

        var hasChanges = !JsonSerializer.Serialize(appData.Roles).Equals(JsonSerializer.Serialize(updatedRoles));
        
        appData.Roles = updatedRoles;

        return hasChanges;
    }

    private static bool SynchronizePermissions(AppDataModel appData, IReadOnlySet<string> permissionsFromCode)
    {
        var originalPermissionCount = appData.Permissions.Count;
        var originalPermissions = appData.Permissions.ToDictionary(p => p.Name);

        var updatedPermissions = new List<PermissionDetailModel>();
        var newPermissionsAdded = false;

        foreach (var permissionName in permissionsFromCode.OrderBy(p => p))
        {
            if (originalPermissions.TryGetValue(permissionName, out var existingPermission))
            {
                updatedPermissions.Add(existingPermission);
            }
            else
            {
                updatedPermissions.Add(new PermissionDetailModel { Name = permissionName });
                newPermissionsAdded = true;
            }
        }

        appData.Permissions = updatedPermissions;

        return newPermissionsAdded || updatedPermissions.Count != originalPermissionCount;
    }

    private static AppDataModel LoadAppData(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            return new AppDataModel { Version = 0 };
        }

        var json = File.ReadAllText(jsonFilePath);
        return JsonSerializer.Deserialize<AppDataModel>(json) ?? new AppDataModel();
    }
}