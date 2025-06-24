using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TMS.Contract;
using TMS.Contract.CommunicationModels;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHandlers;

public class SubmitTaskCommandHandler(
    IEntityCommiter entityCommiter,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<SubmitTaskCommand, ApiResponse<List<SubmitTaskResponseDto>>>
{
    public async Task<ApiResponse<List<SubmitTaskResponseDto>>> Handle(SubmitTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the current user's id from claims
            var currentUserId = httpContextAccessor.HttpContext?.User?.FindFirst("Employee_Id")?.Value;
            Guid userGuid;
            if (string.IsNullOrEmpty(currentUserId) | Guid.TryParse(currentUserId, out userGuid) is false)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.Unauthorized, "User not authenticated");
            }

            // Get the employee information for the current user
            var employeeResult = await entityCommiter.Employees.GetAsync(
                e => e.User.Id == userGuid);

            if (!employeeResult.IsSuccess || employeeResult.Data == null)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.NotFound, "Employee not found for current user");
            }

            var employee = employeeResult.Data;

            // Get the task
            var taskResult = await entityCommiter.Tasks.GetAsync(
                t => t.TaskUniqueIdentifier == request.Request.TaskGuid);

            if (!taskResult.IsSuccess || taskResult.Data == null)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.NotFound, "Task not found");
            }

            var task = taskResult.Data;

            // Check if the employee is assigned to the task
            if (task.AssignedToEmployeeId != employee.Id)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.Forbidden, "You are not assigned to this task");
            }

            // Create the submission
            var submission = new TaskSubmission
            {
                WorkTaskId = task.Id,
                SubmittedByEmployeeId = employee.Id,
                Description = request.Request.Comment,
                SubmissionDate = DateTime.UtcNow,
                Status = "Pending"
            };

            var addSubmissionResult = await entityCommiter.TaskSubmissions.AddAsync(submission);
            if (!addSubmissionResult.IsSuccess)
            {
                return ApiResponse<List<SubmitTaskResponseDto>>.Failure(HttpStatusCode.InternalServerError, "Failed to create task submission");
            }

            await entityCommiter.CommitAsync(cancellationToken);

            // Handle file uploads
            var uploadResults = new List<SubmitTaskResponseDto>();

            if (request.Request.Files != null && request.Request.Files.Any())
            {
                // Get the web root path from http context
                var webRootPath = httpContextAccessor.HttpContext?.Request?.PathBase.Value ?? string.Empty;
                var uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "uploads", "task-submissions", submission.SubmissionUniqueIdentifier.ToString());

                // Ensure directory exists
                Directory.CreateDirectory(uploadDirectory);

                foreach (var file in request.Request.Files)
                {
                    if (file.Length > 0)
                    {
                        // Generate a unique filename
                        var fileExtension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadDirectory, uniqueFileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream, cancellationToken);
                        }

                        // Create a record in the database
                        var submissionFile = new SubmissionFile
                        {
                            TaskSubmissionId = submission.Id,
                            FileName = uniqueFileName,
                            OriginalFileName = file.FileName,
                            FilePath = Path.Combine("uploads", "task-submissions", submission.SubmissionUniqueIdentifier.ToString(), uniqueFileName),
                            FileExtension = fileExtension,
                            FileSize = file.Length,
                            ContentType = file.ContentType
                        };

                        var addFileResult = await entityCommiter.SubmissionFiles.AddAsync(submissionFile);
                        if (addFileResult.IsSuccess)
                        {
                            // Add to results
                            uploadResults.Add(new SubmitTaskResponseDto
                            {
                                Name = file.Name,
                                ContentType = file.ContentType,
                                Length = file.Length,
                                FileName = file.FileName
                            });
                        }
                    }
                }

                await entityCommiter.CommitAsync(cancellationToken);
            }

            // Update task status if needed
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
