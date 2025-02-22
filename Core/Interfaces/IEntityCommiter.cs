using TMS.Core.Entities;
using TMS.Core.Entities.Interfaces;

namespace TMS.Core.Interfaces;

public interface IEntityCommiter : IDisposable
{
    IDbContextRepository<T> GetRepository<T>() where T : Entity;
    IDbContextRepository<Employee> Employees { get; }
    IDbContextRepository<Department> Departments { get; }
    IDbContextRepository<Claim> Claims { get; }
    IDbContextRepository<Role> Roles { get; }
    IDbContextRepository<WorkTask> Tasks { get; }
    IDbContextRepository<TaskAssignment> TaskAssignments { get; }
    IDbContextRepository<Point> Points { get; }
    int Commit();
    Task<int> CommitAsync();
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
