using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TMS.Application.Commands;
using TMS.Application.Queries;
using TMS.Core.Entities;
using TMS.Core.MediatR;
using TMS.Core.MediatR.Interfaces;
namespace TMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services)
    {
        services.AddEntityRequests<Claim>();
        services.AddEntityRequests<Department>();
        services.AddEntityRequests<Employee>();
        services.AddEntityRequests<Point>();
        services.AddEntityRequests<Role>();
        services.AddEntityRequests<TaskAssignment>();
        services.AddScoped<ISender, Sender>();
        return services;
    }
    public static void AddEntityRequests<T>(this IServiceCollection services) where T : class,IHasId
    {
        services.AddScoped<IRequestHandler<GetAllEntityQuery<T>, DbRequest<List<T>>>, GetAllEntityQueryHandler<T>>();
        services.AddScoped<IRequestHandler<GetAllPaginatedEntityQuery<T>, DbRequest<PaginatedDbRequest<T>>>,GetAllPaginatedEntityQueryHandler<T>>();
        services.AddScoped<IRequestHandler<GetEntityQuery<T>, DbRequest<T>>, GetEntityQueryHandler<T>>();
        services.AddScoped<IRequestHandler<AddEntityCommand<T>, DbRequest>, AddEntityCommandHandler<T>>();
        services.AddScoped<IRequestHandler<UpdateEntityCommand<T>, DbRequest>, UpdateEntityCommandHandler<T>>();
        services.AddScoped<IRequestHandler<DeleteEntityCommand<T>, DbRequest>, DeleteEntityCommandHandler<T>>();
    }
}

