using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;
using TMS.Contract.CommunicationModels;
using System.Net;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers
{
    public record GetTaskSubmissionByIdQuery(Guid SubmissionGuidId) : IRequest<ApiResponse<TaskSubmission>>;

    public class GetTaskSubmissionByIdQueryHandler : IRequestHandler<GetTaskSubmissionByIdQuery, ApiResponse<TaskSubmission>>
    {
        private readonly IEntityCommiter _commiter;
        public GetTaskSubmissionByIdQueryHandler(IEntityCommiter commiter) => _commiter = commiter;

        public async Task<ApiResponse<TaskSubmission>> Handle(GetTaskSubmissionByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _commiter.TaskSubmissions.GetAsync(x => x.SubmissionUniqueIdentifier == request.SubmissionGuidId);
            return result.IsSuccess && result.Data != null
                ? ApiResponse<TaskSubmission>.Success(result.Data, HttpStatusCode.OK, "Task submission retrieved.")
                : ApiResponse<TaskSubmission>.Failure(HttpStatusCode.NotFound, result.Message ?? "Submission not found.");
        }
    }
} 