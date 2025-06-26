using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
namespace TMS.Application.Services.Interfaces.TaskServices;

public interface IAddTaskService
{
    Task<ApiResponse<AddTaskResponseDto>> AddTask(AddTaskDto dto, CancellationToken cancellationToken);
} 