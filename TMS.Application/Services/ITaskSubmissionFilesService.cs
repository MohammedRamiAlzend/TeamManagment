using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;
namespace TMS.Application.Services;

public interface ITaskSubmissionFilesService
{
    Task<ApiResponse<List<SubmissionFileDto>>> GetTaskSubmissionFiles(GetTaskSubmissionFilesQuery request, CancellationToken cancellationToken);
} 