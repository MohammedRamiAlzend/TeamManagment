using TMS.Application.Services.Interfaces.ProjectInterfaces;

namespace TMS.Application.Services.ProjectServices;

public class ProjectService : IProjectService
{
    private readonly IEntityCommiter _commiter;
    public ProjectService(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<List<Employee>> GetEnrolledMembers(List<int> enrolledMembersIds)
    {
        var employees = new List<Employee>();
        foreach (var id in enrolledMembersIds)
        {
            var employee = await _commiter.Employees.GetAsync(x => x.Id == id);
            if(employee.IsSuccess && employee.Data is not null)
                employees.Add(employee.Data);
        }
        return employees;
    }

    public async Task<List<WorkTask>> GetTasksForProject(List<Guid> tasksIds)
    {
        var tasks = new List<WorkTask>();
        foreach (var id in tasksIds)
        {
            var task = await _commiter.Tasks.GetAsync(x => x.TaskUniqueIdentifier == id);
            if(task.IsSuccess && task.Data is not null)
                tasks.Add(task.Data);
        }
        return tasks;
    }
} 