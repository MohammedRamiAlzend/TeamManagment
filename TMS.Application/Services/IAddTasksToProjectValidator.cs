namespace TMS.Application.Services;

public interface IAddTasksToProjectValidator
{
    Task<DbRequest> Validate(List<Guid> taskIds);
} 