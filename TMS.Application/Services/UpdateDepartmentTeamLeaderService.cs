using Microsoft.EntityFrameworkCore;

namespace TMS.Application.Services;

public class UpdateDepartmentTeamLeaderService : IUpdateDepartmentTeamLeaderService
{
    private readonly IEntityCommiter _entityCommiter;
    private readonly ILogger<UpdateDepartmentTeamLeaderService> _logger;
    public UpdateDepartmentTeamLeaderService(IEntityCommiter entityCommiter, ILogger<UpdateDepartmentTeamLeaderService> logger)
    {
        _entityCommiter = entityCommiter;
        _logger = logger;
    }

    public async Task<ApiResponse> UpdateTeamLeader(int departmentId, int teamLeaderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateDepartmentTeamLeaderService is running");
        var getDepartment = await _entityCommiter.Departments.GetAsync(
            filter: department => department.Id == departmentId,
            include: x => x.Include(i => i.TeamLeader).ThenInclude(tl => tl.User.Roles));
        if (!getDepartment.IsSuccess || getDepartment.Data is null)
            return ApiResponse.Failure(HttpStatusCode.NotFound,
                getDepartment.Message ?? $"Department with id {departmentId} not found");
        var getEmployee = await _entityCommiter.Employees.GetAsync(
           filter: x => x.Id == teamLeaderId,
           include: x => x.Include(i => i.User.Roles));
        if (!getEmployee.IsSuccess || getEmployee.Data is null)
            return ApiResponse.Failure(HttpStatusCode.NotFound,
                getEmployee.Message ?? $"Employee with id {teamLeaderId} not found");
        var department = getDepartment.Data;
        var newTeamLeader = getEmployee.Data;
        if (newTeamLeader.User is null)
            return ApiResponse.Failure(HttpStatusCode.BadRequest, "Employee user information is missing");
        if (newTeamLeader.User.Roles is null)
            newTeamLeader.User.Roles = new List<Role>();
        if (department.TeamLeaderId == newTeamLeader.Id)
            return ApiResponse.Success(HttpStatusCode.OK, "Team leader is already assigned to this department");
        var existingTeamLeaderAssignment = await _entityCommiter.Departments.GetAsync(
            filter: d => d.TeamLeaderId == newTeamLeader.Id && d.Id != departmentId);
        if (existingTeamLeaderAssignment.IsSuccess && existingTeamLeaderAssignment.Data is not null)
        {
            return ApiResponse.Failure(HttpStatusCode.Conflict,
                $"Employee with id {teamLeaderId} is already assigned as team leader to department with id {existingTeamLeaderAssignment.Data.Id}");
        }
        var getRoles = await _entityCommiter.Roles.GetAllAsync();
        if (!getRoles.IsSuccess || getRoles.Data is null)
        {
            return ApiResponse.Failure(HttpStatusCode.BadRequest, "No roles found");
        }
        var teamLeaderRole = getRoles.Data.FirstOrDefault(r => r.Name == AppRoles.TeamLeader.Name);
        if (teamLeaderRole is null)
            return ApiResponse.Failure(HttpStatusCode.BadRequest, "TeamLeader role not found in system");
        if (department.TeamLeaderId.HasValue && department.TeamLeader?.User?.Roles != null)
        {
            var previousTeamLeader = department.TeamLeader;
            var otherDepartmentsWithSameTeamLeader = await _entityCommiter.Departments.GetAllAsync(
                filter: d => d.TeamLeaderId == previousTeamLeader.Id && d.Id != departmentId);
            if (!otherDepartmentsWithSameTeamLeader.IsSuccess ||
                otherDepartmentsWithSameTeamLeader.Data == null ||
                !otherDepartmentsWithSameTeamLeader.Data.Any())
            {
                var teamLeaderRoleToRemove = previousTeamLeader.User.Roles
                    .FirstOrDefault(r => r.Name.Equals(AppRoles.TeamLeader.Name, StringComparison.OrdinalIgnoreCase));
                if (teamLeaderRoleToRemove != null)
                {
                    previousTeamLeader.User.Roles.Remove(teamLeaderRoleToRemove);
                    _logger.LogInformation($"Removed TeamLeader role from previous team leader (Employee ID: {previousTeamLeader.Id}). Other roles preserved.");
                }
            }
            else
            {
                _logger.LogInformation($"Previous team leader (Employee ID: {previousTeamLeader.Id}) is still a team leader in other departments, keeping TeamLeader role");
            }
        }
        department.TeamLeaderId = newTeamLeader.Id;
        var result = await _entityCommiter.Departments.UpdateAsync(department);
        if (!result.IsSuccess)
            return ApiResponse.Failure(HttpStatusCode.BadRequest, result.Message ?? "Failed to update department");
        var currentRoleNames = newTeamLeader.User.Roles.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!currentRoleNames.Contains(AppRoles.TeamLeader.Name))
        {
            newTeamLeader.User.Roles.Add(teamLeaderRole);
            _logger.LogInformation($"Added TeamLeader role to new team leader (Employee ID: {newTeamLeader.Id})");
        }
        try
        {
            var changesResult = await _entityCommiter.CommitAsync(cancellationToken);
            return changesResult > 0
                ? ApiResponse.Success(HttpStatusCode.OK, "Team leader updated successfully")
                : ApiResponse.Failure(HttpStatusCode.InternalServerError, "No changes were saved to the database");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update exception when updating team leader");
            return ApiResponse.Failure(HttpStatusCode.Conflict,
                "The selected employee is already assigned as team leader to another department");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when updating team leader");
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, "An unexpected error occurred");
        }
    }
} 