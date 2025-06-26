using Microsoft.AspNetCore.Http;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Application.Services;

namespace TMS.Application.Handlers.CustomHandlers.TaskHandlers;

public class SubmitTaskCommandHandler : IRequestHandler<SubmitTaskCommand, ApiResponse<List<SubmitTaskResponseDto>>>
{
    private readonly IEntityCommiter _entityCommiter;
    private readonly IUserContextService _userContextService;
    private readonly ITaskSubmissionFileService _fileService;

    public SubmitTaskCommandHandler(
        IEntityCommiter entityCommiter,
        IUserContextService userContextService,
        ITaskSubmissionFileService fileService)
    {
        _entityCommiter = entityCommiter;
        _userContextService = userContextService;
        _fileService = fileService;
    }

    public async Task<ApiResponse<List<SubmitTaskResponseDto>>> Handle(SubmitTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _userContextService.GetCurrentEmployeeAsync();
            if (employee == null)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.Unauthorized, "User not authenticated or employee not found");
            }

            var taskResult = await _entityCommiter.Tasks.GetAsync(
                t => t.TaskUniqueIdentifier == request.TaskGuid);

            if (!taskResult.IsSuccess || taskResult.Data == null)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.NotFound, "Task not found");
            }

            var task = taskResult.Data;

            if (task.AssignedToEmployeeId != employee.Id)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.Forbidden, "You are not assigned to this task");
            }

            var submission = new TaskSubmission
            {
                WorkTaskId = task.Id,
                SubmittedByEmployeeId = employee.Id,
                Description = request.Request.Comment,
                SubmissionDate = DateTime.UtcNow,
                Status = "Pending",
                FeedbackComments = ""
            };

            var addSubmissionResult = await _entityCommiter.TaskSubmissions.AddAsync(submission);
            if (!addSubmissionResult.IsSuccess)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.InternalServerError, "Failed to create task submission");
            }

            await _entityCommiter.CommitAsync(cancellationToken);

            List<SubmitTaskResponseDto> uploadResults = new();
            if (request.Request.Files != null && request.Request.Files.Any())
            {
                uploadResults = await _fileService.SaveSubmissionFiles(submission, request.Request.Files, cancellationToken);
            }

            task.Status = "Submitted";
            await _entityCommiter.Tasks.UpdateAsync(task);
            await _entityCommiter.CommitAsync(cancellationToken);

            return ApiResponse<List<SubmitTaskResponseDto>>.Success(uploadResults, HttpStatusCode.OK, "Task submitted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}
