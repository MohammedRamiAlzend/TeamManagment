using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
namespace TMS.Application.Services.Interfaces.TaskInterfaces;

public interface ISubmissionFilesZippingService
{
    Task<ApiResponse<ZipSubmissionFileResult>> GetAllSubmissionsFilesZip(GetAllSubmissionsFilesQuery request, CancellationToken cancellationToken);
} 