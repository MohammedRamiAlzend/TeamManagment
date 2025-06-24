using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.CommunicationModels;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;

public class GetTaskSubmissionFilesQueryHandler : IRequestHandler<GetTaskSubmissionFilesQuery, ApiResponse<List<SubmissionFileDto>>>
{
    private readonly IEntityCommiter _entityCommiter;
    private readonly ILogger<GetTaskSubmissionFilesQueryHandler> _logger;

    public GetTaskSubmissionFilesQueryHandler(
        IEntityCommiter entityCommiter,
        ILogger<GetTaskSubmissionFilesQueryHandler> logger)
    {
        _entityCommiter = entityCommiter;
        _logger = logger;
    }

    public async Task<ApiResponse<List<SubmissionFileDto>>> Handle(GetTaskSubmissionFilesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // First verify the task exists
            var taskResult = await _entityCommiter.Tasks.GetAsync(
                t => t.TaskUniqueIdentifier == request.TaskId);

            if (!taskResult.IsSuccess || taskResult.Data == null)
            {
                return ApiResponse<List<SubmissionFileDto>>.Failure(HttpStatusCode.NotFound, "Task not found");
            }

            // Find the submission
            var submissionResult = await _entityCommiter.TaskSubmissions.GetAsync(
                s => s.SubmissionUniqueIdentifier == request.SubmissionId,
                include: s => s.Files);

            if (!submissionResult.IsSuccess || submissionResult.Data == null)
            {
                return ApiResponse<List<SubmissionFileDto>>.Failure(HttpStatusCode.NotFound, "Submission not found");
            }

            var submission = submissionResult.Data;

            // Verify submission belongs to the specified task
            if (submission.WorkTaskId != taskResult.Data.Id)
            {
                return ApiResponse<List<SubmissionFileDto>>.Failure(HttpStatusCode.BadRequest, "Submission does not belong to the specified task");
            }

            // Convert files to DTOs
            var filesList = new List<SubmissionFileDto>();
            if (submission.Files != null)
            {
                foreach (var file in submission.Files)
                {
                    filesList.Add(new SubmissionFileDto
                    {
                        Id = file.Id,
                        FileName = file.FileName,
                        OriginalFileName = file.OriginalFileName,
                        ContentType = file.ContentType,
                        FileSize = file.FileSize,
                        UploadedDate = file.CreatedAt
                    });
                }
            }

            return ApiResponse<List<SubmissionFileDto>>.Success(filesList, HttpStatusCode.OK, "Files retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving submission files");
            return ApiResponse<List<SubmissionFileDto>>.Failure(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}
