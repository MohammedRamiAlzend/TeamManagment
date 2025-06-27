using Microsoft.EntityFrameworkCore;

namespace TMS.Contract;

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
            .ThenInclude(d => d.ParentDepartment)!
            .Include(e=>e.TaskSubmissions)
            .ThenInclude(e=>e.Files);
    }
    
    
    public static Func<IQueryable<Project>, IIncludableQueryable<Project, object>>? IncludeProjectRelations()
    {
        return query => query
            .Include(e => e.TeamMembers)
            .ThenInclude(x => x.User)
            .ThenInclude(x => x.Roles)
            .Include(e => e.Department)
            .ThenInclude(x => x.ParentDepartment)
            .Include(e => e.Tasks)
            .ThenInclude(x => x.AssignedTo)
            .Include(e => e.Tasks)
            .ThenInclude(x => x.CreatedBy)!;
    }
    public static Func<IQueryable<WorkTask>, IIncludableQueryable<WorkTask, object>>? IncludeTaskRelations()
    {
        return query => query
            .Include(e => e.CreatedBy)
            .ThenInclude(e=>e.User)
            .ThenInclude(e=>e.Roles)
            .Include(e => e.AssignedTo)
            .ThenInclude(e=>e.User)
            .ThenInclude(e=>e.Roles)
            .Include(e=>e.Projects)
            .ThenInclude(x=>x.Department)
            .Include(x=>x.Submissions)
            .ThenInclude(x=>x.Files);
    }

    public static Func<IQueryable<TaskSubmission>, IIncludableQueryable<TaskSubmission, object>>? IncludeTaskSubmittionsRelations()
    {
        return query => query
            .Include(e => e.WorkTask)
            .Include(e=>e.SubmittedBy)
            .Include(e=>e.Files);
    }

}