using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;
using TMS.Application.Services.Interfaces.TaskInterfaces;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;

public class GetTaskSubmissionFilesQueryHandler : IRequestHandler<GetTaskSubmissionFilesQuery, ApiResponse<List<SubmissionFileDto>>>
{
    private readonly ITaskSubmissionFilesService _service;
    public GetTaskSubmissionFilesQueryHandler(ITaskSubmissionFilesService service)
    {
        _service = service;
    }

    public async Task<ApiResponse<List<SubmissionFileDto>>> Handle(GetTaskSubmissionFilesQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetTaskSubmissionFiles(request, cancellationToken);
    }
}
