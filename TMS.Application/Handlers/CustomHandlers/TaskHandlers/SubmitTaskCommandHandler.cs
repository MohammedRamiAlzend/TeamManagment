using Microsoft.AspNetCore.Http;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Application.Handlers.CustomHandlers.TaskHandlers;

public class SubmitTaskCommandHandler(
    IEntityCommiter entityCommiter,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<SubmitTaskCommand, ApiResponse<List<SubmitTaskResponseDto>>>
{
    public async Task<ApiResponse<List<SubmitTaskResponseDto>>> Handle(SubmitTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = httpContextAccessor.HttpContext?.User?.FindFirst("Employee_Id")?.Value;
            
            if (string.IsNullOrEmpty(currentUserId) | Guid.TryParse(currentUserId, out var userGuid) is false)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.Unauthorized, "User not authenticated");
            }

            var employeeResult = await entityCommiter.Employees.GetAsync(
                e => e.User.Id == userGuid);

            if (!employeeResult.IsSuccess || employeeResult.Data == null)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.NotFound, "Employee not found for current user");
            }

            var employee = employeeResult.Data;

            var taskResult = await entityCommiter.Tasks.GetAsync(
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
                FeedbackComments = "" // Adding empty string as default value to prevent NULL constraint violation
            };

            var addSubmissionResult = await entityCommiter.TaskSubmissions.AddAsync(submission);
            if (!addSubmissionResult.IsSuccess)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.InternalServerError, "Failed to create task submission");
            }

           var x = await entityCommiter.CommitAsync(cancellationToken);

            var uploadResults = new List<SubmitTaskResponseDto>();

            if (request.Request.Files != null &&  request.Request.Files.Any())
            {
                var webRootPath = httpContextAccessor.HttpContext?.Request?.PathBase.Value ?? string.Empty;
                var uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "task-submissions", submission.SubmissionUniqueIdentifier.ToString());

                Directory.CreateDirectory(uploadDirectory);
                foreach( var file in request.Request.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadDirectory, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream, cancellationToken);
                        }

                        var submissionFile = new SubmissionFile
                        {
                            TaskSubmissionId = submission.Id,
                            FileName = uniqueFileName,
                            OriginalFileName = file.FileName,
                            FilePath = Path.Combine("uploads", "task-submissions",
                                submission.SubmissionUniqueIdentifier.ToString(), uniqueFileName),
                            FileExtension = fileExtension,
                            FileSize = file.Length,
                            ContentType = file.ContentType
                        };

                        var addFileResult = await entityCommiter.SubmissionFiles.AddAsync(submissionFile);
                        if (addFileResult.IsSuccess)
                        {
                            uploadResults.Add(new SubmitTaskResponseDto
                            {
                                FileGuidId = submissionFile.FileUniqueIdentifier,
                                Name = submissionFile.FileName,
                                ContentType = submissionFile.ContentType,
                                Length = submissionFile.FileSize,
                                FileName = submissionFile.OriginalFileName
                            });
                        }
                    }

                    await entityCommiter.CommitAsync(cancellationToken);
                }
            }

            task.Status = "Submitted";
            await entityCommiter.Tasks.UpdateAsync(task);
            await entityCommiter.CommitAsync(cancellationToken);

            return ApiResponse<List<SubmitTaskResponseDto>>.Success(uploadResults, HttpStatusCode.OK, "Task submitted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}
