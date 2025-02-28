using TMS.Application.Commands;
using TMS.Application.Queries;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs;
using TMS.Core.Entities.Interfaces;

namespace TMS.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services)
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
    public static void AddEntityRequests<TEntity, TEntityDTO>(this IServiceCollection services) where TEntity : Entity
                                                                                               where TEntityDTO : IDTO
    {
        services.AddScoped<IRequestHandler<GetAllEntityQuery<TEntity, TEntityDTO>, ApiResponse>, GetAllEntityQueryHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<GetAllPaginatedEntityQuery<TEntity, TEntityDTO>, PaginatedApiResponse>, GetAllPaginatedEntityQueryHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<GetEntityQuery<TEntity, TEntityDTO>, ApiResponse>, GetEntityQueryHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<AddEntityCommand<TEntityDTO>, ApiResponse>, AddEntityCommandHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<UpdateEntityCommand<TEntity, TEntityDTO>, ApiResponse>, UpdateEntityCommandHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<DeleteEntityCommand<TEntity>, ApiResponse>, DeleteEntityCommandHandler<TEntity>>();
    }
}

