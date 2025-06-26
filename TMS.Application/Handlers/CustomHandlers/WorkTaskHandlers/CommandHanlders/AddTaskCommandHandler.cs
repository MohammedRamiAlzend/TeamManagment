using Microsoft.Extensions.Logging;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.Entities.Interfaces;
using TMS.Core.Interfaces;
using TMS.Application.Services;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders;

public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, ApiResponse<AddTaskResponseDto>>
{
    private readonly IAddTaskService _service;
    public AddTaskCommandHandler(IAddTaskService service)
    {
        _service = service;
    }

    public async Task<ApiResponse<AddTaskResponseDto>> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        return await _service.AddTask(request.Dto, cancellationToken);
    }
}
