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
    public record GetTaskSubmissionByIdQuery(Guid SubmissionGuidId) : IRequest<ApiResponse<TaskSubmissionResponseDto>>;

    public class GetTaskSubmissionByIdQueryHandler : IRequestHandler<GetTaskSubmissionByIdQuery, ApiResponse<TaskSubmissionResponseDto>>
    {
        private readonly IEntityCommiter _commiter;
        private readonly IMapper _mapper;
        public GetTaskSubmissionByIdQueryHandler(IEntityCommiter commiter, IMapper mapper)
        {
            _commiter = commiter;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TaskSubmissionResponseDto>> Handle(GetTaskSubmissionByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _commiter.TaskSubmissions.GetAsync(x => x.SubmissionUniqueIdentifier == request.SubmissionGuidId
            ,include: i=> i.Include(x=>x.WorkTask)
                            .Include(x=>x.Files)
                            .Include(x=>x.SubmittedBy));
            if (!result.IsSuccess || result.Data == null)
                return ApiResponse<TaskSubmissionResponseDto>.Failure(HttpStatusCode.NotFound, result.Message ?? "Submission not found.");
            var dto = _mapper.Map<TaskSubmissionResponseDto>(result.Data);
            return ApiResponse<TaskSubmissionResponseDto>.Success(dto, HttpStatusCode.OK, "Task submission retrieved.");
        }
    }
} 