using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;
using TMS.Contract.CommunicationModels;
using System.Net;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders
{
    public class AddTaskSubmissionCommand : IRequest<ApiResponse<TaskSubmission>>
    {
        public TaskSubmission Submission { get; set; }
        public AddTaskSubmissionCommand(TaskSubmission submission) => Submission = submission;
    }

    public class AddTaskSubmissionCommandHandler : IRequestHandler<AddTaskSubmissionCommand, ApiResponse<TaskSubmission>>
    {
        private readonly IEntityCommiter _commiter;
        public AddTaskSubmissionCommandHandler(IEntityCommiter commiter) => _commiter = commiter;

        public async Task<ApiResponse<TaskSubmission>> Handle(AddTaskSubmissionCommand request, CancellationToken cancellationToken)
        {
            var addResult = await _commiter.TaskSubmissions.AddAsync(request.Submission);
            await _commiter.CommitAsync(cancellationToken);
            return addResult.IsSuccess
                ? ApiResponse<TaskSubmission>.Success(request.Submission, HttpStatusCode.Created, "Task submission created.")
                : ApiResponse<TaskSubmission>.Failure(HttpStatusCode.BadRequest, addResult.Message ?? "Failed to create task submission.");
        }
    }
} 