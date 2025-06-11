using TMS.Core.CommunicationModels;
using TMS.Core.Entities;
using TMS.Infrastructure.Data.DbContextTools;
using TMS.Infrastructure.DataSeeder.Interfaces;

namespace TMS.Infrastructure.DataSeeder.Seeders;

public class DepartmentSeeder : IDataSeeder
{
    public EnvironmentEnum Environment => EnvironmentEnum.All;
    public int Priority => 5;

    public async Task<DbRequest> SeedAsync(AppDbContext context)
    {
        if (await context.Departments.AnyAsync()) return DbRequest.Success("Nothing To add");
        try
        {
            var departmentsList = new List<Department>();
            var getTeamLeader = context.Users.Include(user => user.Employee)
                .FirstOrDefault(x => x.Roles.Any(y => y.Name.ToLower() == "teamleader"));
            if (getTeamLeader is null) return DbRequest.Failure("TeamLeader not found");
            var getEmployees = await context.Users.Where(x => x.Roles.Any(y => y.Name.ToLower() == "employee"))
                .Select(x => x.Employee).ToListAsync();
            departmentsList.Add(new Department
            {
                Name = "ITSupport",
                TeamLeader = getTeamLeader.Employee,
                Employees = getEmployees,
                Email = "ITSupport@mail.com",
                PhoneNumber = "123"
            });
            departmentsList.Add(new Department
            {
                Name = "HR",
                TeamLeader = getTeamLeader.Employee,
                Employees = getEmployees,
                Email = "HR@mail.com",
                PhoneNumber = "124"
            });
            departmentsList.Add(new Department
            {
                Name = "Programming",
                TeamLeader = getTeamLeader.Employee,
                Employees = getEmployees,
                Email = "Programming@mail.com",
                PhoneNumber = "124"
            });

            await context.AddRangeAsync(departmentsList);

            var result = await context.SaveChangesAsync();

            return result > 0 ? DbRequest.Success() : DbRequest.Failure("Error according to permissions");
        }
        catch (Exception e)
        {
            return DbRequest.Failure(e.Message);
        }
    }
}