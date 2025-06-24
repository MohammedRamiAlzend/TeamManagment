using TMS.Application.Handlers.CustomHandlers.ProjectHandlers;
using TMS.Application.Handlers.CustomHandlers.TaskHandlers;
using TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

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
        services.AddEntityGetRegistration<Department,GetDepartmentResponse>();
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
        services.AddRequestHandler<UpdateProjectCommand, ApiResponse<UpdateProjectDto>, UpdateProjectCommandHandler>();
        services.AddRequestHandler<DeleteProjectCommand, ApiResponse<bool>, DeleteProjectCommandHandler>();
        
        //Task Registration
        services.AddEntityGetRegistration<WorkTask,GetTaskResponse>();
        services.AddEntityAdditionRegistration<WorkTask,AddTaskDto>();
        services.AddEntityUpdationRegistration<WorkTask,UpdateTaskDto>();
        services.AddEntityDeletionRegistration<WorkTask>();
        services.AddRequestHandler<UpdateWorkTaskCommand, ApiResponse, UpdateTaskCommandHandler>();
        services.AddRequestHandler<SubmitTaskCommand, ApiResponse<List<SubmitTaskResponseDto>>, SubmitTaskCommandHandler>();
        services.AddRequestHandler<GetTaskSubmissionFilesQuery, ApiResponse<List<SubmissionFileDto>>, GetTaskSubmissionFilesQueryHandler>();
        services.AddRequestHandler<GetSubmissionFileQuery, ApiResponse<SubmissionFileResult>, GetSubmissionFileQueryHandler>();
        services.AddRequestHandler<GetAllSubmissionsFilesQuery, ApiResponse<ZipSubmissionFileResult>, GetAllSubmissionsFilesQueryHandler>();
         
            
            
        services.AddScoped<ISender, Sender>();
        services.AddHttpContextAccessor();
        
        return services;
    }
}

