using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
namespace TMS.Application.Services;

public interface IAddTaskService
{
    Task<ApiResponse<AddTaskResponseDto>> AddTask(AddTaskDto dto, CancellationToken cancellationToken);
} 