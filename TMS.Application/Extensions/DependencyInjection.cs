using TMS.Application.Handlers.CustomHandlers.EmployeeHandlers;
using TMS.Application.Handlers.CustomHandlers.ProjectHandlers;
using TMS.Application.Handlers.CustomHandlers.TaskHandlers;
using TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders;
using TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;
using TMS.Application.Services.DepartmentServices;
using TMS.Application.Services.EmployeeServices;
using TMS.Application.Services.Interfaces.DepartmentInterfaces;
using TMS.Application.Services.Interfaces.EmployeeInterfaces;
using TMS.Application.Services.Interfaces.ProjectInterfaces;
using TMS.Application.Services.Interfaces.TaskInterfaces;
using TMS.Application.Services.Interfaces.TaskServices;
using TMS.Application.Services.ProjectServices;
using TMS.Application.Services.TaskServices;
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
        // services.AddEntityAdditionRegistration<Department, CreateDepartmentDto>();
        services.AddEntityGetRegistration<Department,GetDepartmentResponse>();
        services.AddRequestHandler<UpdateDepartmentTeamLeaderCommand, ApiResponse, UpdateDepartmentTeamLeaderHandler>();
        services.AddRequestHandler<CreateDepartmentCommand, ApiResponse, CreateDepartmentCommandHandler>();
        services.AddRequestHandler<UpdateDepartmentCommand, ApiResponse, UpdateDepartmentCommandHandler>();
        services.AddRequestHandler<DeleteDepartmentCommand,ApiResponse,DeleteDepartmentCommandHandler>();

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
        services.AddRequestHandler<AddTasksToProjectCommand, ApiResponse, AddTasksToProjectCommandHandler>();
        
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
        services.AddRequestHandler<AddTaskCommand, ApiResponse<AddTaskResponseDto>, AddTaskCommandHandler>();
        services.AddRequestHandler<DeleteEmployeeCommand, bool, DeleteEmployeeCommandHandler>();
        services.AddRequestHandler<AddTaskSubmissionCommand, ApiResponse<TaskSubmission>, AddTaskSubmissionCommandHandler>();
        services.AddRequestHandler<UpdateTaskSubmissionCommand, ApiResponse<TaskSubmission>, UpdateTaskSubmissionCommandHandler>();
        services.AddRequestHandler<DeleteTaskSubmissionCommand, ApiResponse, DeleteTaskSubmissionCommandHandler>();
        services.AddRequestHandler<GetTaskSubmissionsQuery, ApiResponse<List<TaskSubmission>>, GetTaskSubmissionsQueryHandler>();
        services.AddRequestHandler<GetTaskSubmissionByIdQuery, ApiResponse<TaskSubmission>, GetTaskSubmissionByIdQueryHandler>();
            
            
        services.AddScoped<ISender, Sender>();
        services.AddHttpContextAccessor();
        services.AddScoped<IDepartmentValidator, DepartmentValidator>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ITaskSubmissionFileService, TaskSubmissionFileService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IProjectValidator, ProjectValidator>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IAddTasksToProjectValidator, AddTasksToProjectValidator>();
        services.AddScoped<IAddTasksToProjectService, AddTasksToProjectService>();
        services.AddScoped<ICreateDepartmentValidator, CreateDepartmentValidator>();
        services.AddScoped<ICreateDepartmentService, CreateDepartmentService>();
        services.AddScoped<IUpdateDepartmentTeamLeaderService, UpdateDepartmentTeamLeaderService>();
        services.AddScoped<IAddTaskService, AddTaskService>();
        services.AddScoped<IUpdateWorkTaskService, UpdateWorkTaskService>();
        services.AddScoped<ISubmissionFilesZippingService, SubmissionFilesZippingService>();
        services.AddScoped<ISubmissionFileRetrievalService, SubmissionFileRetrievalService>();
        services.AddScoped<ITaskSubmissionFilesService, TaskSubmissionFilesService>();
        
        return services;
    }
}

