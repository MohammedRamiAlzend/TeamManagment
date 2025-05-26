using TMS.Core.Entities;
using TMS.Infrastructure.AppConfigurations;
using TMS.Infrastructure.Data.DbContextTools;
using TMS.Infrastructure.DataSeeder.Interfaces;

namespace TMS.Infrastructure.DataSeeder.Seeders;

public class RoleSeeder : IDataSeeder
{
    public int Priority => 2;
    public EnvironmentEnum Environment => EnvironmentEnum.All;
    public async Task<DbRequest> SeedAsync(AppDbContext context)
    {
        try
        {
            if (await context.Roles.AnyAsync()) return DbRequest.Success("Nothing To add");
            
            var roles = PermissionSettings.Roles.Select(x => new Role { Name = x.Name});
            
            await context.Roles.AddRangeAsync(roles);
            var result = await context.SaveChangesAsync();
            
            return result > 0 ? DbRequest.Success() : DbRequest.Failure("Error according to roles");
        }
        catch (Exception e)
        {
            return DbRequest.Failure(e.Message);
        }
    }
}