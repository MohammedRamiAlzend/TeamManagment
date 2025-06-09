using TMS.Core.CommunicationModels;
using TMS.Infrastructure.Data.DbContextTools;
using TMS.Infrastructure.DataSeeder.Interfaces;

namespace TMS.Infrastructure.DataSeeder.Seeders;

public class LinkPermissionsToRolesSeeder : IDataSeeder
{
    public EnvironmentEnum Environment => EnvironmentEnum.All;
    public int Priority => 3;

    public async Task<DbRequest> SeedAsync(AppDbContext context)
    {
        try
        {
            if (await context.Permissions.AnyAsync(x => x.Roles.Any())) return DbRequest.Success("nothing to add");

            var getAdminRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == "admin");
            var getManagerRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == "manager");
            var getObserverRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == "observer");
            var getEmployeeRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == "employee");
            var getTeamLeaderRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == "teamleader");
            if (getAdminRole == null || getManagerRole == null || getEmployeeRole == null || getObserverRole == null ||
                getTeamLeaderRole == null)
                return DbRequest.Failure("Roles not found");

            getAdminRole.Permissions = await context.Permissions.ToListAsync();
            getManagerRole.Permissions = await context.Permissions.ToListAsync();
            getObserverRole.Permissions =
                await context.Permissions.Where(x => x.Name.ToLower().Contains("get")).ToListAsync();
            getEmployeeRole.Permissions =
                await context.Permissions.Where(x => x.Name.ToLower().Contains("get")).ToListAsync();
            getTeamLeaderRole.Permissions = await context.Permissions.Where(x =>
                    x.Name.ToLower().Contains("usermanagement") ||
                    x.Name.ToLower().Contains("projectmanagement") ||
                    x.Name.ToLower().Contains("taskmanagement"))
                .ToListAsync();
            context.Update(getAdminRole);
            var result = await context.SaveChangesAsync();

            return result > 0 ? DbRequest.Success() : DbRequest.Failure("Error according to permissions");
        }
        catch (Exception e)
        {
            return DbRequest.Failure(e.Message);
        }
    }
}