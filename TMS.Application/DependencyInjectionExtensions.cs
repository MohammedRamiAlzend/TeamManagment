using TMS.Application.GenericCommands;
using TMS.Application.GenericQueries;

namespace TMS.Application;

public static class DependencyInjectionExtensions
{
        public static void AddEntityRegistration<TEntity, TDto>(
        this IServiceCollection services) 
        where TEntity : Entity 
        where TDto : IDto
    {
        services.AddRequestHandler<GetAllEntityQuery<TEntity, TDto>, ApiResponse<List<TDto>>, GetAllEntityQueryHandler<TEntity, TDto>>();
        services.AddRequestHandler<GetAllPaginatedEntityQuery<TEntity, TDto>, PaginatedApiResponse<TDto>, GetAllPaginatedEntityQueryHandler<TEntity, TDto>>();
        services.AddRequestHandler<GetEntityQuery<TEntity, TDto>, ApiResponse<TDto>, GetEntityQueryHandler<TEntity, TDto>>();
        services.AddRequestHandler<UpdateEntityCommand<TEntity, TDto>, ApiResponse<TDto>, UpdateEntityCommandHandler<TEntity, TDto>>();
        services.AddRequestHandler<AddEntityCommand<TDto>, ApiResponse<TDto>, AddEntityCommandHandler<TEntity, TDto>>();
        services.AddRequestHandler<DeleteEntityCommand<TEntity>, ApiResponse, DeleteEntityCommandHandler<TEntity>>();
    }
    public static void AddEntityGetQueryRegistration<TEntity, TDto>(
        this IServiceCollection services) 
        where TEntity : Entity 
        where TDto : IDto
    {
        services.AddRequestHandler<GetEntityQuery<TEntity, TDto>, ApiResponse<TDto>, GetEntityQueryHandler<TEntity, TDto>>();
    }
    public static void AddEntityGetAllQueryRegistration<TEntity, TDto>(
        this IServiceCollection services) 
        where TEntity : Entity 
        where TDto : IDto
    {
        services.AddRequestHandler<GetAllEntityQuery<TEntity, TDto>, ApiResponse<List<TDto>>, GetAllEntityQueryHandler<TEntity, TDto>>();
    }
    
    public static void AddEntityGetAllPaginatedQueryRegistration<TEntity, TDto>(
        this IServiceCollection services) 
        where TEntity : Entity 
        where TDto : IDto
    {
        services.AddRequestHandler<GetAllPaginatedEntityQuery<TEntity, TDto>, PaginatedApiResponse<TDto>, GetAllPaginatedEntityQueryHandler<TEntity, TDto>>();
    }

    public static void AddEntityDeletionRegistration<TEntity, TDto>(
        this IServiceCollection services) 
        where TEntity : Entity 
        where TDto : IDto
    {
        services.AddRequestHandler<DeleteEntityCommand<TEntity>, ApiResponse, DeleteEntityCommandHandler<TEntity>>();
    }
    
    public static void AddEntityAdditionRegistration<TEntity, TDto>(
        this IServiceCollection services) 
        where TEntity : Entity 
        where TDto : IDto
    {
        services.AddRequestHandler<AddEntityCommand<TDto>, ApiResponse<TDto>, AddEntityCommandHandler<TEntity, TDto>>();
    }
    public static void AddEntityUpdationRegistration<TEntity, TDto>(
        this IServiceCollection services) 
        where TEntity : Entity 
        where TDto : IDto
    {
        services.AddRequestHandler<UpdateEntityCommand<TEntity, TDto>, ApiResponse<TDto>, UpdateEntityCommandHandler<TEntity, TDto>>();
    }
    public static void AddRequestHandler<TRequest, TResponse, THandler>(
        this IServiceCollection services) 
        where TRequest : IRequest<TResponse> 
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
    }
}