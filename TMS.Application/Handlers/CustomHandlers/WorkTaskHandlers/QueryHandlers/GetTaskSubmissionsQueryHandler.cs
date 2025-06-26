using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;
using TMS.Contract.CommunicationModels;
using System.Net;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers
{
    public record GetTaskSubmissionsQuery(Guid WorkTaskGuidId) : IRequest<ApiResponse<List<TaskSubmission>>>;

    public class GetTaskSubmissionsQueryHandler : IRequestHandler<GetTaskSubmissionsQuery, ApiResponse<List<TaskSubmission>>>
    {
        private readonly IEntityCommiter _commiter;
        public GetTaskSubmissionsQueryHandler(IEntityCommiter commiter) => _commiter = commiter;

        public async Task<ApiResponse<List<TaskSubmission>>> Handle(GetTaskSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var getTask = await _commiter.Tasks.AnyAsync(x => x.TaskUniqueIdentifier == request.WorkTaskGuidId);
            var result = await _commiter.TaskSubmissions.GetAllAsync(x => x.WorkTask.TaskUniqueIdentifier == request.WorkTaskGuidId,
                include:QueryIncludeHelper.IncludeTaskSubmittionsRelations());
            return result.IsSuccess
                ? ApiResponse<List<TaskSubmission>>.Success(result.Data!, HttpStatusCode.OK, "Task submissions retrieved.")
                : ApiResponse<List<TaskSubmission>>.Failure(HttpStatusCode.NotFound, result.Message ?? "No submissions found.");
        }
    }
} 