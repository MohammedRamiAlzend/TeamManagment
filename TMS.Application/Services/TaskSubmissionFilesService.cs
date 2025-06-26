using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

namespace TMS.Application.Services;

public class TaskSubmissionFilesService : ITaskSubmissionFilesService
{
    private readonly IEntityCommiter _entityCommiter;
    private readonly ILogger<TaskSubmissionFilesService> _logger;
    public TaskSubmissionFilesService(IEntityCommiter entityCommiter, ILogger<TaskSubmissionFilesService> logger)
    {
        _entityCommiter = entityCommiter;
        _logger = logger;
    }

    public async Task<ApiResponse<List<SubmissionFileDto>>> GetTaskSubmissionFiles(GetTaskSubmissionFilesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var taskResult = await _entityCommiter.Tasks.GetAsync(
                t => t.TaskUniqueIdentifier == request.TaskId);
            if (!taskResult.IsSuccess || taskResult.Data == null)
            {
                return ApiResponse<List<SubmissionFileDto>>.Failure(HttpStatusCode.NotFound, "Task not found");
            }
            var task = taskResult.Data;
            var submissionResult = await _entityCommiter.TaskSubmissions.GetAllAsync(
                s => s.WorkTaskId == task.Id,
                include: s =>
                    s.Include(i => i.Files)
                        .Include(x => x.WorkTask));
            if (!submissionResult.IsSuccess || submissionResult.Data == null || !submissionResult.Data.Any())
            {
                return ApiResponse<List<SubmissionFileDto>>.Failure(HttpStatusCode.NotFound, "No submissions found for this task");
            }
            var submissions = submissionResult.Data;
            var filesList = new List<SubmissionFileDto>();
            foreach (var submission in submissions)
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