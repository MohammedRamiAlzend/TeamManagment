using TMS.Application.CQRS.Commands.DepartmentCommands;
using TMS.Application.Handlers.DepartmentHandlers;

namespace TMS.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services)
    {
        // Employee Registration
        services.AddEntityAdditionRegistration<Employee, CreateEmployeeDto>();
        services.AddEntityUpdationRegistration<Employee, UpdateEmployeeDto>();
        services.AddEntityDeletionRegistration<Employee>();
        services.AddEntityGetAllQueryRegistration<Employee, GetEmployeeResponse>();
        services.AddEntityGetQueryRegistration<Employee, GetEmployeeResponse>();
        services.AddEntityGetAllPaginatedQueryRegistration<Employee, GetEmployeeResponse>();
        // services.AddEntityRegistration<Employee, EmployeeDto>();

        //Department Registration
        services.AddEntityAdditionRegistration<Department, CreateDepartmentDto>();
        services.AddEntityUpdationRegistration<Department, UpdateDepartmentDto>();
        services.AddEntityRegistration<Department, DepartmentDto>();
        services.AddEntityGetQueryRegistration<Department, GetDepartmentResponse>();
        services.AddEntityGetAllQueryRegistration<Department, GetDepartmentResponse>();
        services.AddEntityGetAllPaginatedQueryRegistration<Department, GetDepartmentResponse>();
        services.AddRequestHandler<UpdateDepartmentTeamLeaderCommand, ApiResponse, UpdateDepartmentTeamLeaderHandler>();
        
        //User Registration
        services.AddRequestHandler<RegisterUserCommand, ApiResponse<User>, RegisterUserCommandHandler>();
        services.AddRequestHandler<LoginUserCommand, ApiResponse<TokenResponseDto>, LoginUserCommandHandler>();
        services.AddRequestHandler<RefreshTokenCommand, ApiResponse<TokenResponseDto>, RefreshTokenCommandHandler>();
        
        //Permission Registration
        services.AddEntityGetAllQueryRegistration<Permission,GetPermissionResponse>();
        services.AddEntityGetQueryRegistration<Permission,GetPermissionResponse>();
        services.AddEntityGetAllPaginatedQueryRegistration<Permission,GetPermissionResponse>();
        
        services.AddScoped<ISender, Sender>();

        return services;
    }
}