using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
namespace TMS.Application.Services.Interfaces.TaskInterfaces;

public interface IUpdateWorkTaskService
{
    Task<ApiResponse> UpdateWorkTask(UpdateWorkTaskCommand request, CancellationToken cancellationToken);
} 