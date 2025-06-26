using Microsoft.AspNetCore.Http;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Application.Services;

public class TaskSubmissionFileService : ITaskSubmissionFileService
{
    private readonly IEntityCommiter _entityCommiter;
    public TaskSubmissionFileService(IEntityCommiter entityCommiter)
    {
        _entityCommiter = entityCommiter;
    }

    public async Task<List<SubmitTaskResponseDto>> SaveSubmissionFiles(TaskSubmission submission, IEnumerable<IFormFile> files, CancellationToken cancellationToken)
    {
        var uploadResults = new List<SubmitTaskResponseDto>();
        var uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "task-submissions", submission.SubmissionUniqueIdentifier.ToString());
        Directory.CreateDirectory(uploadDirectory);
        foreach (var file in files)
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

                var addFileResult = await _entityCommiter.SubmissionFiles.AddAsync(submissionFile);
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
            await _entityCommiter.CommitAsync(cancellationToken);
        }
        return uploadResults;
    }
} 