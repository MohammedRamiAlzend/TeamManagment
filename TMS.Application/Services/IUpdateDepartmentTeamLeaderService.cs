using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;

namespace TMS.Application.Services;

public interface IUpdateDepartmentTeamLeaderService
{
    Task<ApiResponse> UpdateTeamLeader(int departmentId, int teamLeaderId, CancellationToken cancellationToken);
} 