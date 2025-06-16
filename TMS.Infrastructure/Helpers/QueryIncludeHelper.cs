namespace TMS.Infrastructure.Helpers;

public static class QueryIncludeHelper
{
    public static Func<IQueryable<Department>, IIncludableQueryable<Department, object>>? IncludeDepartmentRelations()
    {
        return query => query
            .Include(d => d.TeamLeader)!
            .ThenInclude(tl => tl.User)
            .Include(x=>x.Employees)
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments)!.ThenInclude(sd => sd.TeamLeader);
    }
    
    public static Func<IQueryable<Employee>, IIncludableQueryable<Employee, object>>? IncludeEmployeeRelations()
    {
        return query => query
            .Include(e => e.User).ThenInclude(x=>x.Roles)
            .Include(e=>e.AssignedTasks)
            .Include(e=>e.CreatedTasks)
            .Include(e=>e.Projects)
            .Include(e => e.Departments)
            .ThenInclude(d => d.ParentDepartment)!;
    }

}