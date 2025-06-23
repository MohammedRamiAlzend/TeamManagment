using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;

public class DeleteProjectCommandHandler(IEntityCommiter commiter) : IRequestHandler<DeleteProjectCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await commiter.Projects.RemoveAsync(x=>x.Id==request.ProjectId);
        if (deleteResult.IsSuccess is false)
            return ApiResponse<bool>.Failure(HttpStatusCode.NotFound, deleteResult.Message);

        var commitResult = await commiter.CommitAsync(cancellationToken);

        return commitResult > 0
            ? ApiResponse<bool>.Success(true)
            : ApiResponse<bool>.Failure(HttpStatusCode.InternalServerError, "Failed to delete project.");
    }
} 