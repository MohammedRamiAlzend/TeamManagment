using TMS.Core.Entities.Interfaces;

namespace TMS.Core.Interfaces;

public interface IEntityCommiter : IDisposable
{
    IDbContextRepository<T> GetRepository<T>() where T :  Entity;
    IDbContextRepository<Employee> Employees { get; }
    IDbContextRepository<Department> Departments { get; }
    IDbContextRepository<Permission> Claims { get; }
    IDbContextRepository<Role> Roles { get; }
    IDbContextRepository<WorkTask> Tasks { get; }
    int Commit();
    Task<int> CommitAsync();
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
