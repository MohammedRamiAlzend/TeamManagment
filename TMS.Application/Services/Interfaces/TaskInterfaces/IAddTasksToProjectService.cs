namespace TMS.Application.Services.Interfaces.TaskServices;

public interface IAddTasksToProjectService
{
    Task<List<WorkTask>> GetTasksForProject(List<Guid> tasksIds);
} 