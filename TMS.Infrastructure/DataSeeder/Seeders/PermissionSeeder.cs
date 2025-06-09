using TMS.Core.CommunicationModels;
using TMS.Core.Entities;
using TMS.Infrastructure.AppConfigurations;
using TMS.Infrastructure.Data.DbContextTools;
using TMS.Infrastructure.DataSeeder.Interfaces;

namespace TMS.Infrastructure.DataSeeder.Seeders;

public class PermissionSeeder : IDataSeeder
{
    public EnvironmentEnum Environment => EnvironmentEnum.All;
    public int Priority => 1;

    public async Task<DbRequest> SeedAsync(AppDbContext context)
    {
        try
        {
            if (await context.Permissions.AnyAsync()) return DbRequest.Success("Nothing To add");

            var permissions = PermissionSettings.Permissions.Select(x => new Permission
                { Name = x.Name, Description = x.Description });

            await context.Permissions.AddRangeAsync(permissions);
            var result = await context.SaveChangesAsync();

            return result > 0 ? DbRequest.Success() : DbRequest.Failure("Error according to permissions");
        }
        catch (Exception e)
        {
            return DbRequest.Failure(e.Message);
        }
    }
}