namespace TMS.Application.Services;

public interface IAddTasksToProjectService
{
    Task<List<WorkTask>> GetTasksForProject(List<Guid> tasksIds);
} 