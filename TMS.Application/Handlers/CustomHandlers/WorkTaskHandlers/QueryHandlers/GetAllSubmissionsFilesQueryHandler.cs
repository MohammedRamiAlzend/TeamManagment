using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Application.Services;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;

public class GetAllSubmissionsFilesQueryHandler : IRequestHandler<GetAllSubmissionsFilesQuery, ApiResponse<ZipSubmissionFileResult>>
{
    private readonly ISubmissionFilesZippingService _zippingService;
    public GetAllSubmissionsFilesQueryHandler(ISubmissionFilesZippingService zippingService)
    {
        _zippingService = zippingService;
    }

    public async Task<ApiResponse<ZipSubmissionFileResult>> Handle(GetAllSubmissionsFilesQuery request, CancellationToken cancellationToken)
    {
        return await _zippingService.GetAllSubmissionsFilesZip(request, cancellationToken);
    }
}
