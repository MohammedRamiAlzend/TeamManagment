using TMS.Application.Commands.AuthCommands;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestDepartmentsDTOS;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestEmployeeDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseDepartmentDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseEmployeeDTOS;
using TMS.Core.Entities.Models;

namespace TMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services)
    {
        // Employee Registration
        services.AddEntityAdditionRegistration<Employee, CreateEmployeeDto>();
        services.AddEntityUpdationRegistration<Employee, UpdateEmployeeDto>();
        services.AddEntityRegistration<Employee, EmployeeDto>();
        
        //Department Registration
        services.AddEntityAdditionRegistration<Department,CreateDepartmentDto>();
        services.AddEntityUpdationRegistration<Department,UpdateDepartmentDto>();
        services.AddEntityRegistration<Department,DepartmentDto>();
        
        services.AddRequestHandler<RegisterUserCommand,ApiResponse<User>,RegisterUserCommandHandler>();
        services.AddRequestHandler<LoginUserCommand,ApiResponse<TokenResponseDto>,LoginUserCommandHandler>();
        services.AddRequestHandler<RefreshTokenCommand,ApiResponse<TokenResponseDto>,RefreshTokenCommandHandler>();
        
        
        
        
        services.AddScoped<ISender, Sender>();

        return services;
    }
}