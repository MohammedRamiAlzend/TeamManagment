using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TMS.Application.Services.Interfaces.TaskInterfaces;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CommunicationModels;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

namespace TMS.Application.Services.TaskServices;

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
            //var taskResult = await _entityCommiter.Tasks.GetAsync(
            //    t => t.TaskUniqueIdentifier == request.TaskGuidId);
            //if (!taskResult.IsSuccess || taskResult.Data == null)
            //{
            //    return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "Task not found");
            //}
            //var task = taskResult.Data;
            var submissionResult = await _entityCommiter.SubmissionFiles.GetAllAsync(x=>x.FileUniqueIdentifier == request.FileGuidId);
            if (!submissionResult.IsSuccess || submissionResult.Data == null || !submissionResult.Data.Any())
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "No submissions found for this task");
            }
            // Find the file among all submissions
            var getFileResult = await _entityCommiter.SubmissionFiles.GetAsync(f => f.FileUniqueIdentifier == request.FileGuidId);
            if (getFileResult.IsSuccess && getFileResult.Data is not null)
            {
                var file = getFileResult.Data;
                if (file == null)
                {
                    return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "File not found");
                }
                var filePath = file.FilePath;
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
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
            else
            {
                return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.NotFound, "File content not found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving submission file");
            return ApiResponse<SubmissionFileResult>.Failure(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}