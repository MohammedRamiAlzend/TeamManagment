using MediatR;
using System;
using TMS.Contract.CommunicationModels;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

namespace TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;

public record GetTaskSubmissionFilesQuery(Guid TaskId, Guid SubmissionId) : IRequest<ApiResponse<List<SubmissionFileDto>>>;
