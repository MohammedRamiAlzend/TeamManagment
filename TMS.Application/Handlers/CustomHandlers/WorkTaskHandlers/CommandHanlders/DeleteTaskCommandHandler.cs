using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CommunicationModels;
using TMS.Core.Interfaces;
using TMS.Contract.Entities;
using System.Net;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, ApiResponse>
{
    private readonly IEntityCommiter _commiter;
    public DeleteTaskCommandHandler(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<ApiResponse> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        // Find the WorkTask by Guid
        var workTaskResult = await _commiter.Tasks.GetAsync(x => x.TaskUniqueIdentifier == request.TaskGuidId);
        if (!workTaskResult.IsSuccess || workTaskResult.Data == null)
            return ApiResponse.Failure(HttpStatusCode.NotFound, $"Task with Guid {request.TaskGuidId} not found.");
        var workTask = workTaskResult.Data;

        // Find and delete all related TaskSubmissions
        var submissionsResult = await _commiter.TaskSubmissions.GetAllAsync(x => x.WorkTaskId == workTask.Id);
        if (submissionsResult.IsSuccess && submissionsResult.Data != null)
        {
            foreach (var submission in submissionsResult.Data)
            {
                await _commiter.TaskSubmissions.RemoveAsync(x=>x.Id == submission.Id);
            }
        }

        // Delete the WorkTask
        var deleteResult = await _commiter.Tasks.RemoveAsync(x => x.Id == workTask.Id);
        await _commiter.CommitAsync(cancellationToken);
        return deleteResult.IsSuccess
            ? ApiResponse.Success(HttpStatusCode.OK, "Task and related submissions deleted successfully.")
            : ApiResponse.Failure(HttpStatusCode.BadRequest, deleteResult.Message ?? "Failed to delete task.");
    }
} 