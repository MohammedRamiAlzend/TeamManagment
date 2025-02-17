using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data.Entities;

namespace Core.Repositories;

public interface IEntityCommiter : IDisposable
{
    IDbContextRepository<T> GetRepository<T>() where T : class;
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
