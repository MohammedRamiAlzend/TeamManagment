using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;

namespace TMS.Application.Services;

public class SubmissionFileRetrievalService : ISubmissionFileRetrievalService
{
    private readonly IEntityCommiter _entityCommiter;
    private readonly ILogger<SubmissionFileRetrievalService> _logger;
    public SubmissionFileRetrievalService(IEntityCommiter entityCommiter, ILogger<SubmissionFileRetrievalService> logger)
    {
        _entityCommiter = entityCommiter;
        _logger = logger;
    }

    public async Task<ApiResponse<SubmissionFileResult>> GetSubmissionFile(GetSubmissionFileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var taskResult = await _entityCommiter.Tasks.GetAsync(
                t => t.TaskUniqueIdentifier == request.TaskId);
            if (!taskResult.IsSuccess || taskResult.Data == null)
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "Task not found");
            }
            var task = taskResult.Data;
            var submissionResult = await _entityCommiter.TaskSubmissions.GetAllAsync(
                filter: s => s.WorkTaskId == task.Id,
                include: s => s.Include(i => i.Files));
            if (!submissionResult.IsSuccess || submissionResult.Data == null || !submissionResult.Data.Any())
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "No submissions found for this task");
            }
            var submission = submissionResult.Data.FirstOrDefault(x => x.Id == request.FileId);
            if (submission is null)
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "File not found");
            }
            var fileResult = await _entityCommiter.SubmissionFiles.GetAsync(
                f => f.Id == request.FileId && f.TaskSubmissionId == submission.Id);
            if (!fileResult.IsSuccess || fileResult.Data == null)
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "File not found");
            }
            var file = fileResult.Data;
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "task-submissions", file.TaskSubmission.SubmissionUniqueIdentifier.ToString(), file.FileName);
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