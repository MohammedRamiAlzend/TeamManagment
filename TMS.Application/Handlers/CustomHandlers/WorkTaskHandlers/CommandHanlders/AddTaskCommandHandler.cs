using Microsoft.Extensions.Logging;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.Entities.Interfaces;
using TMS.Core.Interfaces;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders;

public class AddTaskCommandHandler(IEntityCommiter commiter, IMapper mapper,ILogger<AddTaskCommand> logger) : IRequestHandler<AddTaskCommand, ApiResponse<AddTaskResponseDto>>
{
    public async Task<ApiResponse<AddTaskResponseDto>> Handle(AddTaskCommand request,
    CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing AddEntityCommand for {EntityType}", typeof(AddTaskDto).Name);

        var entity = MapEntity(request.Dto);
        if (entity == null)
            return ApiResponse<AddTaskResponseDto>.Failure(HttpStatusCode.InternalServerError,
                "Entity mapping resulted in null.");
        var addingResult = await AddEntityToRepositoryAsync(entity, request.Dto, cancellationToken);
        if (addingResult.IsSuccess)
        {
            var x2 = mapper.Map<AddTaskResponseDto>(entity);
            return ApiResponse<AddTaskResponseDto>.Success(x2, HttpStatusCode.OK);
        }
        else return ApiResponse<AddTaskResponseDto>.Failure(HttpStatusCode.InternalServerError, addingResult.Message);
    }


    private async Task<ApiResponse<AddTaskDto>> AddEntityToRepositoryAsync(WorkTask entity, AddTaskDto entityDto,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Adding Entity of type {EntityType} to the repository", typeof(WorkTask).Name);

            var repository = commiter.Tasks;
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(WorkTask).Name);
                return ApiResponse<AddTaskDto>.Failure(HttpStatusCode.InternalServerError,
                    "Repository is unavailable.");
            }

            var requestAdd = await repository.AddAsync(entity);
            if (!requestAdd.IsSuccess)
            {
                logger.LogError("Repository addition failed for {EntityType}: {Message}", typeof(WorkTask).Name,
                    requestAdd.Message);
                return ApiResponse<AddTaskDto>.Failure(HttpStatusCode.BadRequest, requestAdd.Message??"");
            }

            await commiter.CommitAsync(cancellationToken);
            logger.LogInformation("Entity added successfully: {Message}", requestAdd.Message);
            return ApiResponse<AddTaskDto>.Success(entityDto, HttpStatusCode.Created, requestAdd.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding Entity of type {EntityType}", typeof(WorkTask).Name);
            return ApiResponse<AddTaskDto>.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while saving the entity.");
        }
        finally
        {
            logger.LogInformation("AddEntityCommand processing completed for {EntityType}", typeof(AddTaskDto).Name);
        }
    }
    private WorkTask MapEntity(AddTaskDto entityDto)
    {
        try
        {
            logger.LogInformation("Mapping DTO to Entity for {EntityType}", typeof(AddTaskDto).Name);
            return mapper.Map<WorkTask>(entityDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to map DTO to Entity for {EntityType}", typeof(AddTaskDto).Name);
            throw;
        }
    }

}
