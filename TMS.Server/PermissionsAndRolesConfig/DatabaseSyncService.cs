using TMS.Contract;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TMS.Server.PermissionsAndRolesConfig;


public class DatabaseSyncService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DatabaseSyncService> _logger;

    public DatabaseSyncService(IServiceProvider serviceProvider, IHostEnvironment environment, ILogger<DatabaseSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _environment = environment;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Database Sync Service is starting.");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var jsonFilePath = Path.Combine(_environment.ContentRootPath, ConfigHelper.PermissionsRolesFileName);
        if (!File.Exists(jsonFilePath))
        {
            _logger.LogError($"{ConfigHelper.PermissionsRolesFileName} file not found. Synchronization cannot proceed.");
            return;
        }

        try
        {
            var jsonData = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
            var appData = JsonSerializer.Deserialize<AppDataModel>(jsonData);

            if (appData == null)
            {
                _logger.LogError($"Failed to deserialize {ConfigHelper.PermissionsRolesFileName}. Synchronization halted.");
                return;
            }
            
            _logger.LogInformation("Synchronizing Permissions...");
            await SyncPermissionsAsync(context, appData.Permissions);

            _logger.LogInformation("Synchronizing Roles...");
            await SyncRolesAsync(context, appData.Roles);

            _logger.LogInformation("Linking Permissions to Roles...");
            await LinkRolesAndPermissionsAsync(context, appData.Roles);
            
            _logger.LogInformation("Database synchronization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database synchronization.");
        }
    }

    private async Task SyncPermissionsAsync(AppDbContext context, List<PermissionDetailModel> permissionsFromFile)
    {
        var dbPermissions = await context.Permissions.ToDictionaryAsync(p => p.Name);
        var permissionsFromFileSet = permissionsFromFile.ToDictionary(p => p.Name);

        foreach (var permFromFile in permissionsFromFile)
        {
            if (dbPermissions.TryGetValue(permFromFile.Name, out _))
            {
            }
            else
            {
                context.Permissions.Add(new Permission { Name = permFromFile.Name });
            }
        }

        var permissionsToRemove = dbPermissions.Values.Where(p => !permissionsFromFileSet.ContainsKey(p.Name));
        context.Permissions.RemoveRange(permissionsToRemove);

        await context.SaveChangesAsync();
    }


    private async Task SyncRolesAsync(AppDbContext context, List<RoleModel> rolesFromFile)
    {
        var dbRoleNames = await context.Roles.Select(r => r.Name).ToHashSetAsync();
        var roleNamesFromFile = rolesFromFile.Select(r => r.Name).ToHashSet();
        
        var rolesToAdd = rolesFromFile
            .Where(r => !dbRoleNames.Contains(r.Name))
            .Select(r => new Role { Name = r.Name });
        
        await context.Roles.AddRangeAsync(rolesToAdd);
        
        var rolesToRemove = await context.Roles.Where(r => !roleNamesFromFile.Contains(r.Name)).ToListAsync();
        if (rolesToRemove.Any())
        {
            context.Roles.RemoveRange(rolesToRemove);
        }

        await context.SaveChangesAsync();
    }

    private async Task LinkRolesAndPermissionsAsync(AppDbContext context, List<RoleModel> rolesFromFile)
    {
        var allDbRoles = await context.Roles.Include(r => r.Permissions).ToListAsync();
        var allDbPermissions = await context.Permissions.ToDictionaryAsync(p => p.Name);

        foreach (var roleFromFile in rolesFromFile)
        {
            var dbRole = allDbRoles.FirstOrDefault(r => r.Name == roleFromFile.Name);
            if (dbRole == null) continue;

            var requiredPermissionNames = roleFromFile.Permissions.Select(p => p.Name).ToHashSet();
            
            var currentPermissionNames = dbRole.Permissions.Select(p => p.Name).ToHashSet();

            var permissionsToAdd = requiredPermissionNames.Where(name => !currentPermissionNames.Contains(name) && allDbPermissions.ContainsKey(name));
            var permissionsToRemove = currentPermissionNames.Where(name => !requiredPermissionNames.Contains(name));

            foreach (var permName in permissionsToAdd)
            {
                dbRole.Permissions.Add(allDbPermissions[permName]);
            }
            
            foreach (var permName in permissionsToRemove)
            {
                 var permToRemove = dbRole.Permissions.First(p => p.Name == permName);
                 dbRole.Permissions.Remove(permToRemove);
            }
        }

        await context.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
