using TMS.Application.Handlers.CustomHandlers.ProjectHandlers;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

namespace TMS.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services)
    {
        // Employee Registration
        services.AddEntityUpdationRegistration<Employee, UpdateEmployeeDto>();
        services.AddEntityDeletionRegistration<Employee>();
        services.AddEntityGetRegistration<Employee, GetEmployeeResponse>();

        //Department Registration
        services.AddEntityAdditionRegistration<Department, CreateDepartmentDto>();
        services.AddEntityUpdationRegistration<Department, UpdateDepartmentCommand>();
        services.AddEntityGetRegistration<Department,GetDepartmentQuery>();
        services.AddRequestHandler<UpdateDepartmentTeamLeaderCommand, ApiResponse, UpdateDepartmentTeamLeaderHandler>();

        
        //User Registration
        services.AddRequestHandler<RegisterUserCommand, ApiResponse<User>, RegisterUserCommandHandler>();
        services.AddRequestHandler<LoginUserCommand, ApiResponse<TokenResponseDto>, LoginUserCommandHandler>();
        services.AddRequestHandler<RefreshTokenCommand, ApiResponse<TokenResponseDto>, RefreshTokenCommandHandler>();
        
        //Permission Registration
        services.AddEntityGetRegistration<Permission,GetPermissionResponse>();

        //Role Registration
        services.AddEntityGetRegistration<Role,GetRoleResponse>();
        
        //project Registration
        services.AddEntityGetRegistration<Project,GetProjectResponse>();
        services.AddRequestHandler<AddProjectCommand,ApiResponse<AddProjectDto>,AddProjectHandler>();
        services.AddScoped<ISender, Sender>();

        return services;
    }
}