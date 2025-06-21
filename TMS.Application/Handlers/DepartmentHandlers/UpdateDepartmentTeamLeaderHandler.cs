using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.Data.SqlClient;
using TMS.Application.CQRS.Commands.DepartmentCommands;
using TMS.Contract;

namespace TMS.Application.Handlers.DepartmentHandlers;

public class UpdateDepartmentTeamLeaderHandler(
    IEntityCommiter entityCommiter,
    ILogger<UpdateDepartmentTeamLeaderHandler> logger
    ) : IRequestHandler<UpdateDepartmentTeamLeaderCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateDepartmentTeamLeaderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateDepartmentTeamLeaderHandler is running");
        
        var getDepartment = await entityCommiter.Departments.GetAsync(
            filter: department => department.Id == request.DepartmentId,
            include: x => x.Include(i => i.TeamLeader));
        
        if (!getDepartment.IsSuccess || getDepartment.Data is null)
            return ApiResponse.Failure(HttpStatusCode.NotFound, 
                getDepartment.Message ?? $"Department with id {request.DepartmentId} not found");
        
        var getEmployee = await entityCommiter.Employees.GetAsync(
           filter: x => x.Id == request.TeamLeaderId,
           include: x => x.Include(i => i.User.Roles));
        
        if (!getEmployee.IsSuccess || getEmployee.Data is null)
            return ApiResponse.Failure(HttpStatusCode.NotFound, 
                getEmployee.Message ?? $"Employee with id {request.TeamLeaderId} not found");
        
        var department = getDepartment.Data;
        var employee = getEmployee.Data;
        
        if (department.TeamLeaderId == employee.Id)
            return ApiResponse.Success(HttpStatusCode.OK, "Team leader is already assigned to this department");
        
        var existingTeamLeaderAssignment = await entityCommiter.Departments.GetAsync(
            filter: d => d.TeamLeaderId == employee.Id && d.Id != request.DepartmentId);
        
        if (existingTeamLeaderAssignment.IsSuccess && existingTeamLeaderAssignment.Data is not null)
        {
            return ApiResponse.Failure(HttpStatusCode.Conflict, 
                $"Employee with id {request.TeamLeaderId} is already assigned as team leader to department with id {existingTeamLeaderAssignment.Data.Id}");
        }

        var getRoles = await entityCommiter.Roles.GetAllAsync();

        if (!HasRequiredRoles(employee.User.Roles, "teamleader", "manager"))
            employee.User.Roles.Add(getRoles.Data.First(r => r.Name == AppRoles.TeamLeader.Name));
        
        department.TeamLeaderId = employee.Id;
        var result = await entityCommiter.Departments.UpdateAsync(department);
        
        if (!result.IsSuccess)
            return ApiResponse.Failure(HttpStatusCode.BadRequest, result.Message ?? "Failed to update department");
        
        try
        {
            var changesResult = await entityCommiter.CommitAsync(cancellationToken);
            
            return changesResult > 0 
                ? ApiResponse.Success(HttpStatusCode.OK, "Team leader updated successfully")
                : ApiResponse.Failure(HttpStatusCode.InternalServerError, "No changes were saved to the database");
        }
        catch (DbUpdateException ex) 
        {
            logger.LogError(ex, "Unique constraint violation when updating team leader");
            return ApiResponse.Failure(HttpStatusCode.Conflict, 
                "The selected employee is already assigned as team leader to another department");
        }
    }
    
    private static bool HasRequiredRoles(ICollection<Role> roles, params string[] roleNames)
    {
        if (roles == null || roleNames == null || roleNames.Length == 0)
            return false;
        
        return roles.Any(role => roleNames.Contains(role.Name, StringComparer.OrdinalIgnoreCase));
    }
}