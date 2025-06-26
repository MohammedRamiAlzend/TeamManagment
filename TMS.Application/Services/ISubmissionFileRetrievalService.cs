using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
namespace TMS.Application.Services;

public interface ISubmissionFileRetrievalService
{
    Task<ApiResponse<SubmissionFileResult>> GetSubmissionFile(GetSubmissionFileQuery request, CancellationToken cancellationToken);
} 