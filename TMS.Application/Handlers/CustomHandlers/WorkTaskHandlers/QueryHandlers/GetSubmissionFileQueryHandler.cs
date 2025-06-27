using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Application.Services.Interfaces.TaskInterfaces;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;

public class GetSubmissionFileQueryHandler : IRequestHandler<GetSubmissionFileQuery, ApiResponse<SubmissionFileResult>>
{
    private readonly ISubmissionFileRetrievalService _retrievalService;
    public GetSubmissionFileQueryHandler(ISubmissionFileRetrievalService retrievalService)
    {
        _retrievalService = retrievalService;
    }

    public async Task<ApiResponse<SubmissionFileResult>> Handle(GetSubmissionFileQuery request, CancellationToken cancellationToken)
    {
        return await _retrievalService.GetSubmissionFile(request, cancellationToken);
    }
}
