using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.CommunicationModels;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;

public class GetSubmissionFileQueryHandler : IRequestHandler<GetSubmissionFileQuery, ApiResponse<SubmissionFileResult>>
{
    private readonly IEntityCommiter _entityCommiter;
    private readonly ILogger<GetSubmissionFileQueryHandler> _logger;

    public GetSubmissionFileQueryHandler(
        IEntityCommiter entityCommiter,
        ILogger<GetSubmissionFileQueryHandler> logger)
    {
        _entityCommiter = entityCommiter;
        _logger = logger;
    }

    public async Task<ApiResponse<SubmissionFileResult>> Handle(GetSubmissionFileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // First verify the task exists
            var taskResult = await _entityCommiter.Tasks.GetAsync(
                t => t.TaskUniqueIdentifier == request.TaskId);

            if (!taskResult.IsSuccess || taskResult.Data == null)
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "Task not found");
            }

            // Find the submission
            var submissionResult = await _entityCommiter.TaskSubmissions.GetAsync(
                s => s.SubmissionUniqueIdentifier == request.SubmissionId);

            if (!submissionResult.IsSuccess || submissionResult.Data == null)
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "Submission not found");
            }

            var submission = submissionResult.Data;

            // Verify submission belongs to the specified task
            if (submission.WorkTaskId != taskResult.Data.Id)
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.BadRequest, "Submission does not belong to the specified task");
            }

            // Find the file
            var fileResult = await _entityCommiter.SubmissionFiles.GetAsync(
                f => f.Id == request.FileId && f.TaskSubmissionId == submission.Id);

            if (!fileResult.IsSuccess || fileResult.Data == null)
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "File not found");
            }

            var file = fileResult.Data;

            // Get the file contents
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", file.FilePath);
            if (!File.Exists(filePath))
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "File content not found");
            }

            var fileContents = await File.ReadAllBytesAsync(filePath, cancellationToken);

            return ApiResponse<SubmissionFileResult>.Success(new SubmissionFileResult
            {
                FileContents = fileContents,
                FileName = file.OriginalFileName,
                ContentType = file.ContentType
            }, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving submission file");
            return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}
