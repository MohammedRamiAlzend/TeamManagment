using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;
using TMS.Contract.CommunicationModels;
using System.Net;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders
{
    public record DeleteTaskSubmissionCommand(Guid SubmissionGuidId) : IRequest<ApiResponse>;

    public class DeleteTaskSubmissionCommandHandler : IRequestHandler<DeleteTaskSubmissionCommand, ApiResponse>
    {
        private readonly IEntityCommiter _commiter;
        public DeleteTaskSubmissionCommandHandler(IEntityCommiter commiter) => _commiter = commiter;

        public async Task<ApiResponse> Handle(DeleteTaskSubmissionCommand request, CancellationToken cancellationToken)
        {
            var getResult = await _commiter.TaskSubmissions.GetAsync(x => x.SubmissionUniqueIdentifier == request.SubmissionGuidId);
            if (!getResult.IsSuccess || getResult.Data == null)
                return ApiResponse.Failure(HttpStatusCode.NotFound, "Task submission not found.");

            await _commiter.TaskSubmissions.RemoveAsync(x=>x.Id == getResult.Data.Id );
            await _commiter.CommitAsync(cancellationToken);
            return ApiResponse.Success(HttpStatusCode.OK, "Task submission deleted.");
        }
    }
} 