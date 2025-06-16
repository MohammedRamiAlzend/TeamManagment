namespace TMS.Infrastructure.DataSeeder.Seeders;

public class DepartmentSeeder : IDataSeeder
{
    public EnvironmentEnum Environment => EnvironmentEnum.All;
    public int Priority => 5;

    public async Task<DbRequest> SeedAsync(AppDbContext context)
{
    var x = await context.Departments.AnyAsync();
    if (x) return DbRequest.Success("Nothing To add");
    try
    {
        var departmentsList = new List<Department>();
        
        // Get all team leaders instead of just one
        var teamLeaders = await context.Users
            .Include(user => user.Employee)
            .Where(x => x.Roles.Any(y => y.Name.ToLower() == "teamleader"))
            .ToListAsync();
            
        if (!teamLeaders.Any()) return DbRequest.Failure("No TeamLeaders found");
        
        var getEmployees = await context.Users.Where(x => x.Roles.Any(y => y.Name.ToLower() == "employee"))
            .Select(x => x.Employee).ToListAsync();

        var itSupportLeader = teamLeaders.FirstOrDefault();
        var hrLeader = teamLeaders.Skip(1).FirstOrDefault() ?? teamLeaders.FirstOrDefault();
        var programmingLeader = teamLeaders.Skip(2).FirstOrDefault() ?? teamLeaders.FirstOrDefault();

        if (itSupportLeader?.Employee == null) return DbRequest.Failure("ITSupport TeamLeader Employee not found");
        if (hrLeader?.Employee == null) return DbRequest.Failure("HR TeamLeader Employee not found");
        if (programmingLeader?.Employee == null) return DbRequest.Failure("Programming TeamLeader Employee not found");

        departmentsList.Add(new Department
        {
            Name = "ITSupport",
            TeamLeader = itSupportLeader.Employee,
            Employees = getEmployees,
            ParentDepartment = null,
            TeamLeaderId = itSupportLeader.Employee.Id,
            Email = "ITSupport@mail.com",
            PhoneNumber = "123"
        });
        
        departmentsList.Add(new Department
        {
            Name = "HR",
            TeamLeader = hrLeader.Employee,
            ParentDepartment = null,
            Employees = getEmployees,
            TeamLeaderId = hrLeader.Employee.Id,
            Email = "HR@mail.com",
            PhoneNumber = "124"
        });
        
        departmentsList.Add(new Department
        {
            Name = "Programming",
            TeamLeader = programmingLeader.Employee,
            ParentDepartment = null,
            Employees = getEmployees,
            TeamLeaderId = programmingLeader.Employee.Id,
            Email = "Programming@mail.com",
            PhoneNumber = "125"
        });

        await context.AddRangeAsync(departmentsList);
        var result = await context.SaveChangesAsync();

        return result > 0 ? DbRequest.Success() : DbRequest.Failure("Failed to save departments");
    }
    catch (Exception e)
    {
        return DbRequest.Failure(e.Message);
    }
}

}