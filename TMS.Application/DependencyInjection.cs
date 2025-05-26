using TMS.Application.Commands.AuthCommands;
using TMS.Application.GenericCommands;
using TMS.Application.GenericQueries;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs;

namespace TMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services)
    {
        // Employee Registration
        services.AddEntityAdditionRegistration<Employee, CreateEmployeeDto>();
        services.AddEntityUpdationRegistration<Employee, UpdateEmployeeDto>();
        services.AddEntityRegistration<Employee, EmployeeDto>();
        
        services.AddRequestHandler<RegisterUserCommand,ApiResponse<User>,RegisterUserCommandHandler>();
        
        
        services.AddScoped<ISender, Sender>();

        return services;
    }
}