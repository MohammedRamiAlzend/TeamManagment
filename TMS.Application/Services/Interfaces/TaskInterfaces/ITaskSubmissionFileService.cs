using Microsoft.AspNetCore.Http;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Application.Services.Interfaces.TaskInterfaces;

public interface ITaskSubmissionFileService
{
    Task<List<SubmitTaskResponseDto>> SaveSubmissionFiles(TaskSubmission submission, IEnumerable<IFormFile> files, CancellationToken cancellationToken);
} 