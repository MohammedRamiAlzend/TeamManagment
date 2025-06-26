using Microsoft.Extensions.Logging;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Core.Interfaces;

namespace TMS.Application.Services;

public class AddTaskService : IAddTaskService
{
    private readonly IEntityCommiter _commiter;
    private readonly IMapper _mapper;
    private readonly ILogger<AddTaskService> _logger;
    public AddTaskService(IEntityCommiter commiter, IMapper mapper, ILogger<AddTaskService> logger)
    {
        _commiter = commiter;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<AddTaskResponseDto>> AddTask(AddTaskDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing AddEntityCommand for {EntityType}", typeof(AddTaskDto).Name);
        var entity = MapEntity(dto);
        if (entity == null)
            return ApiResponse<AddTaskResponseDto>.Failure(HttpStatusCode.InternalServerError,
                "Entity mapping resulted in null.");
        var addingResult = await AddEntityToRepositoryAsync(entity, dto, cancellationToken);
        if (addingResult.IsSuccess)
        {
            var x2 = _mapper.Map<AddTaskResponseDto>(entity);
            return ApiResponse<AddTaskResponseDto>.Success(x2, HttpStatusCode.OK);
        }
        else return ApiResponse<AddTaskResponseDto>.Failure(HttpStatusCode.InternalServerError, addingResult.Message);
    }

    private async Task<ApiResponse<AddTaskDto>> AddEntityToRepositoryAsync(WorkTask entity, AddTaskDto entityDto,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding Entity of type {EntityType} to the repository", typeof(WorkTask).Name);
            var repository = _commiter.Tasks;
            if (repository == null)
            {
                _logger.LogError("Repository for {EntityType} is null", typeof(WorkTask).Name);
                return ApiResponse<AddTaskDto>.Failure(HttpStatusCode.InternalServerError,
                    "Repository is unavailable.");
            }
            var requestAdd = await repository.AddAsync(entity);
            if (!requestAdd.IsSuccess)
            {
                _logger.LogError("Repository addition failed for {EntityType}: {Message}", typeof(WorkTask).Name,
                    requestAdd.Message);
                return ApiResponse<AddTaskDto>.Failure(HttpStatusCode.BadRequest, requestAdd.Message ?? "");
            }
            await _commiter.CommitAsync(cancellationToken);
            _logger.LogInformation("Entity added successfully: {Message}", requestAdd.Message);
            return ApiResponse<AddTaskDto>.Success(entityDto, HttpStatusCode.Created, requestAdd.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding Entity of type {EntityType}", typeof(WorkTask).Name);
            return ApiResponse<AddTaskDto>.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while saving the entity.");
        }
        finally
        {
            _logger.LogInformation("AddEntityCommand processing completed for {EntityType}", typeof(AddTaskDto).Name);
        }
    }
    private WorkTask MapEntity(AddTaskDto entityDto)
    {
        try
        {
            _logger.LogInformation("Mapping DTO to Entity for {EntityType}", typeof(AddTaskDto).Name);
            return _mapper.Map<WorkTask>(entityDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to map DTO to Entity for {EntityType}", typeof(AddTaskDto).Name);
            throw;
        }
    }
} 