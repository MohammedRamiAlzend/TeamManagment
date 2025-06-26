namespace TMS.Application.Services;

public class AddTasksToProjectService : IAddTasksToProjectService
{
    private readonly IEntityCommiter _commiter;
    public AddTasksToProjectService(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<List<WorkTask>> GetTasksForProject(List<Guid> tasksIds)
    {
        var tasks = new List<WorkTask>();
        foreach (var id in tasksIds)
        {
            tasks.Add((await _commiter.Tasks.GetAsync(x => x.TaskUniqueIdentifier == id)).Data!);
        }
        return tasks;
    }
} 