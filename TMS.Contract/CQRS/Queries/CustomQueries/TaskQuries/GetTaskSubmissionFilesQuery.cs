using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

namespace TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;

public record GetTaskSubmissionFilesQuery(Guid TaskId) : IRequest<ApiResponse<List<SubmissionFileDto>>>;
