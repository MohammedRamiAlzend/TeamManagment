using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;
using TMS.Contract.CommunicationModels;
using System.Net;
using AutoMapper;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers
{
    public record GetTaskSubmissionsQuery(Guid WorkTaskGuidId) : IRequest<ApiResponse<List<TaskSubmissionResponseDto>>>;

    public class GetTaskSubmissionsQueryHandler : IRequestHandler<GetTaskSubmissionsQuery, ApiResponse<List<TaskSubmissionResponseDto>>>
    {
        private readonly IEntityCommiter _commiter;
        private readonly IMapper _mapper;
        public GetTaskSubmissionsQueryHandler(IEntityCommiter commiter, IMapper mapper)
        {
            _commiter = commiter;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<TaskSubmissionResponseDto>>> Handle(GetTaskSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var getTask = await _commiter.Tasks.AnyAsync(x => x.TaskUniqueIdentifier == request.WorkTaskGuidId);
            var result = await _commiter.TaskSubmissions.GetAllAsync(x => x.WorkTask.TaskUniqueIdentifier == request.WorkTaskGuidId,
                include:QueryIncludeHelper.IncludeTaskSubmittionsRelations());
            if (!result.IsSuccess || result.Data == null)
                return ApiResponse<List<TaskSubmissionResponseDto>>.Failure(HttpStatusCode.NotFound, result.Message ?? "No submissions found.");
            var dtoList = _mapper.Map<List<TaskSubmissionResponseDto>>(result.Data);
            return ApiResponse<List<TaskSubmissionResponseDto>>.Success(dtoList, HttpStatusCode.OK, "Task submissions retrieved.");
        }
    }
} 