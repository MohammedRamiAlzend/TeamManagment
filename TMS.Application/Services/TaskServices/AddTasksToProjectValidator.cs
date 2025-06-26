using TMS.Application.Services.Interfaces.TaskServices;

namespace TMS.Application.Services.TaskServices;

public class AddTasksToProjectValidator : IAddTasksToProjectValidator
{
    private readonly IEntityCommiter _commiter;
    public AddTasksToProjectValidator(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<DbRequest> Validate(List<Guid> taskIds)
    {
        var errorMessage = "";
        foreach (var taskId in taskIds)
        {
            if (await _commiter.Tasks.AnyAsync(x => x.TaskUniqueIdentifier == taskId) is false)
                errorMessage += $"Task with id {taskId} does not exist\n";
        }
        return errorMessage.Length > 0 ? DbRequest.Failure(errorMessage) : DbRequest.Success();
    }
} 