using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
namespace TMS.Application.Services;

public interface ISubmissionFilesZippingService
{
    Task<ApiResponse<ZipSubmissionFileResult>> GetAllSubmissionsFilesZip(GetAllSubmissionsFilesQuery request, CancellationToken cancellationToken);
} 