using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;
using TMS.Contract.CommunicationModels;
using System.Net;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders
{
    public record UpdateTaskSubmissionCommand(Guid SubmissionGuid, TaskSubmission Submission) : IRequest<ApiResponse<TaskSubmission>>;

    public class UpdateTaskSubmissionCommandHandler : IRequestHandler<UpdateTaskSubmissionCommand, ApiResponse<TaskSubmission>>
    {
        private readonly IEntityCommiter _commiter;
        public UpdateTaskSubmissionCommandHandler(IEntityCommiter commiter) => _commiter = commiter;

        public async Task<ApiResponse<TaskSubmission>> Handle(UpdateTaskSubmissionCommand request, CancellationToken cancellationToken)
        {
            var getResult = await _commiter.TaskSubmissions.GetAsync(x => x.SubmissionUniqueIdentifier == request.SubmissionGuid);
            if (!getResult.IsSuccess || getResult.Data == null)
                return ApiResponse<TaskSubmission>.Failure(HttpStatusCode.NotFound, "Task submission not found.");

            // Update properties (add more as needed)
            getResult.Data.Description = request.Submission.Description;
            getResult.Data.Status = request.Submission.Status;
            getResult.Data.FeedbackComments = request.Submission.FeedbackComments;
            getResult.Data.ReviewedDate = request.Submission.ReviewedDate;
            // ... update other properties as needed

            var updateResult = await _commiter.TaskSubmissions.UpdateAsync(getResult.Data);
            await _commiter.CommitAsync(cancellationToken);
            return updateResult.IsSuccess
                ? ApiResponse<TaskSubmission>.Success(getResult.Data, HttpStatusCode.OK, "Task submission updated.")
                : ApiResponse<TaskSubmission>.Failure(HttpStatusCode.BadRequest, updateResult.Message ?? "Failed to update task submission.");
        }
    }
} 