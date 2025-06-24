namespace TMS.Core.Interfaces;

public interface IEntityCommiter : IDisposable
{
    IDbContextRepository<Employee> Employees { get; }
    IDbContextRepository<Department> Departments { get; }
    IDbContextRepository<Permission> Claims { get; }
    IDbContextRepository<Role> Roles { get; }
    IDbContextRepository<WorkTask> Tasks { get; }
    IDbContextRepository<Project> Projects { get; }
    IDbContextRepository<TaskSubmission> TaskSubmissions { get; }
    IDbContextRepository<SubmissionFile> SubmissionFiles { get; }
    
    IDbContextRepository<T> GetRepository<T>() where T : Entity;
    int Commit();
    Task<int> CommitAsync();
    Task<int> CommitAsync(CancellationToken cancellationToken);
}