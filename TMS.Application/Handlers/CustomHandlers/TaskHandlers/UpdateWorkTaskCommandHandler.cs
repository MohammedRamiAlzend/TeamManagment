using Microsoft.EntityFrameworkCore.Query;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using System.Text;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Application.Services;

namespace TMS.Application.Handlers.CustomHandlers.TaskHandlers;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateWorkTaskCommand, ApiResponse>
{
    private readonly IUpdateWorkTaskService _service;
    public UpdateTaskCommandHandler(IUpdateWorkTaskService service)
    {
        _service = service;
    }

    public async Task<ApiResponse> Handle(UpdateWorkTaskCommand request, CancellationToken cancellationToken)
    {
        return await _service.UpdateWorkTask(request, cancellationToken);
    }
}