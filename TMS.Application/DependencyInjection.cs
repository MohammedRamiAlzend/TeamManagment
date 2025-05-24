using TMS.Application.GenericCommands;
using TMS.Application.GenericQueries;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs;
using TMS.Core.Entities.Interfaces;

namespace TMS.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDi(this IServiceCollection services)
    {
        //services.AddEntityRequests<Claim>();
        //services.AddEntityRequests<Department>();
        services.AddEntityRequests<Employee, CreateEmployeeDto>();
        services.AddEntityRequests<Employee, UpdateEmployeeDto>();
        services.AddEntityRequests<Employee, EmployeeDto>();
        //services.AddEntityRequests<Point>();
        //services.AddEntityRequests<Role>();
        //services.AddEntityRequests<TaskAssignment>();
        services.AddScoped<ISender, Sender>();
        return services;
    }
    public static void AddEntityRequests<TEntity, TEntityDto>(this IServiceCollection services) where TEntity : Entity
                                                                                               where TEntityDto : IDto
    {
        services.AddScoped<
            IRequestHandler<GetAllEntityQuery<TEntity, TEntityDto>, ApiResponse>,
                                           GetAllEntityQueryHandler<TEntity, TEntityDto>>();

        services.AddScoped<
            IRequestHandler<GetAllPaginatedEntityQuery<TEntity, TEntityDto>, PaginatedApiResponse>,
                                           GetAllPaginatedEntityQueryHandler<TEntity, TEntityDto>>();

        services.AddScoped<
            IRequestHandler<GetEntityQuery<TEntity, TEntityDto>, ApiResponse>,
                                           GetEntityQueryHandler<TEntity, TEntityDto>>();

        services.AddScoped<
            IRequestHandler<AddEntityCommand<TEntityDto>, ApiResponse>,
                                            AddEntityCommandHandler<TEntity, TEntityDto>>();

        services.AddScoped<
            IRequestHandler<UpdateEntityCommand<TEntity, TEntityDto>, ApiResponse>,
                                           UpdateEntityCommandHandler<TEntity, TEntityDto>>();

        services.AddScoped<
            IRequestHandler<DeleteEntityCommand<TEntity>, ApiResponse>,
                                           DeleteEntityCommandHandler<TEntity>>();
    }
}

