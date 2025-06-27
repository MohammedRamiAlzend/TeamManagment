namespace TMS.Application.Services.Interfaces.TaskServices;

public interface IAddTasksToProjectValidator
{
    Task<DbRequest> Validate(List<Guid> taskIds);
} 