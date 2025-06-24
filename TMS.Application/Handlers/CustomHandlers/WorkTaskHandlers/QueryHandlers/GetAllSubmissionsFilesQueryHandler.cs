using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;

public class GetAllSubmissionsFilesQueryHandler(
    IEntityCommiter entityCommiter,
    ILogger<GetAllSubmissionsFilesQueryHandler> logger)
    : IRequestHandler<GetAllSubmissionsFilesQuery, ApiResponse<ZipSubmissionFileResult>>
{
    public async Task<ApiResponse<ZipSubmissionFileResult>> Handle(GetAllSubmissionsFilesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var submissionFilesBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "task-submissions");
            
            var taskResult = await entityCommiter.Tasks.GetAsync(
                t => t.TaskUniqueIdentifier == request.TaskId);

            if (!taskResult.IsSuccess || taskResult.Data == null)
            {
                return ApiResponse<ZipSubmissionFileResult>.Failure(HttpStatusCode.NotFound, "Task not found");
            }

            var task = taskResult.Data;

            var submissionResult = await entityCommiter.TaskSubmissions.GetAllAsync(
                filter: s => s.WorkTaskId == task.Id,
                include: s => s.Include(i => i.Files));

            if (!submissionResult.IsSuccess || submissionResult.Data == null || !submissionResult.Data.Any())
            {
                return ApiResponse<ZipSubmissionFileResult>.Failure(HttpStatusCode.NotFound, "No submissions found for this task");
            }

            var allSubmissions = submissionResult.Data;
            var allFiles = new List<SubmissionFile>();
            var allFoldersName = new List<string>();
            foreach (var submission in allSubmissions)
            {
                if (submission.Files != null && submission.Files.Any())
                {
                    allFiles.AddRange(submission.Files);
                    allFoldersName.Add(submission.SubmissionUniqueIdentifier.ToString());
                }
            }

            if (!allFiles.Any())
            {
                return ApiResponse<ZipSubmissionFileResult>.Failure(HttpStatusCode.NotFound, "No files found for this task's submissions");
            }

            var outputDirectory = Path.Combine(submissionFilesBasePath, "SavedZips");

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var baseZipFileName = $"Task_{task.TaskUniqueIdentifier}_Files";
            var zipFileExtension = ".zip";

            var zipFilePath = Path.Combine(outputDirectory, baseZipFileName + zipFileExtension);

            int counter = 1;
            while (File.Exists(zipFilePath))
            {
                zipFilePath = Path.Combine(outputDirectory, $"{baseZipFileName}_{counter++}{zipFileExtension}");
            }

            var zipFileName = Path.GetFileName(zipFilePath);

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    int fileCount = 0;
                    foreach (var file in allFiles)
                    {
                        try
                        {
                            var filePath = Path.Combine(submissionFilesBasePath, file.TaskSubmission.SubmissionUniqueIdentifier.ToString(),file.FileName);
                            if (!File.Exists(filePath))
                            {
                                logger.LogWarning("File not found: {FilePath}", filePath);
                                continue;
                            }

                            byte[] fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
                            if (fileBytes.Length == 0)
                            {
                                logger.LogWarning("File is empty: {FilePath}", filePath);
                                continue;
                            }

                            string entryName = $"{++fileCount}_{file.OriginalFileName}";
                            var entry = zipArchive.CreateEntry(entryName);

                            using (var entryStream = entry.Open())
                            {
                                await entryStream.WriteAsync(fileBytes, 0, fileBytes.Length, cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Error adding file {file.OriginalFileName} to zip");
                        }
                    }
                }

                byte[] zipBytes = memoryStream.ToArray();

                // Save the zip file to the output directory
                await File.WriteAllBytesAsync(zipFilePath, zipBytes);

                logger.LogInformation($"Created and saved zip file at: {zipFilePath}");

                return ApiResponse<ZipSubmissionFileResult>.Success(new ZipSubmissionFileResult
                {
                    ZipFileContents = zipBytes,
                    ZipFileName = zipFileName
                }, HttpStatusCode.OK);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving submission file");
            return ApiResponse<ZipSubmissionFileResult>.Failure(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}
