using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs;
using TMS.Core.Entities.Interfaces;

namespace TMS.Core;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services)
    {
        //services.AddEntityRequests<Claim>();
        //services.AddEntityRequests<Department>();
        services.AddEntityRequests<Employee,CreateEmployeeDto>();
        services.AddEntityRequests<Employee,EmployeeDto>();
        //services.AddEntityRequests<Point>();
        //services.AddEntityRequests<Role>();
        //services.AddEntityRequests<TaskAssignment>();
        services.AddScoped<ISender, Sender>();
        return services;
    }
    public static void AddEntityRequests<TEntity,TEntityDTO>(this IServiceCollection services) where TEntity : Entity
                                                                                               where TEntityDTO : IDTO 
    {
        services.AddScoped<IRequestHandler<GetAllEntityQuery<TEntity,TEntityDTO>, DbRequest<List<TEntityDTO>>>, GetAllEntityQueryHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<GetAllPaginatedEntityQuery<TEntity,TEntityDTO>, DbRequest<PaginatedDbRequest<TEntityDTO>>>,GetAllPaginatedEntityQueryHandler<TEntity,TEntityDTO>>();
        services.AddScoped<IRequestHandler<GetEntityQuery<TEntity,TEntityDTO>, DbRequest<TEntityDTO>>, GetEntityQueryHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<AddEntityCommand<TEntityDTO>, DbRequest>, AddEntityCommandHandler<TEntity,TEntityDTO>>();
        services.AddScoped<IRequestHandler<UpdateEntityCommand<TEntity,TEntityDTO>, DbRequest>, UpdateEntityCommandHandler<TEntity, TEntityDTO>>();
        services.AddScoped<IRequestHandler<DeleteEntityCommand<TEntity>, DbRequest>, DeleteEntityCommandHandler<TEntity>>();
    }
}

