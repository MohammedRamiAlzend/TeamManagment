using Microsoft.Extensions.Logging;
using TMS.Core.Entities;
using TMS.Infrastructure.Data.DbContextTools;

namespace TMS.Infrastructure.Repositories;

public class EntityCommiter(AppDbContext appDbContext, ILogger<EntityCommiter> logger) : IEntityCommiter
{
    private readonly Dictionary<Type, object> _repos = [];
    public IDbContextRepository<Employee> Employees => GetRepository<Employee>();
    public IDbContextRepository<Department> Departments => GetRepository<Department>();
    public IDbContextRepository<Permission> Claims => GetRepository<Permission>();
    public IDbContextRepository<Role> Roles => GetRepository<Role>();
    public IDbContextRepository<WorkTask> Tasks => GetRepository<WorkTask>();

    public int Commit()
    {
        return appDbContext.SaveChanges();
    }

    public async Task<int> CommitAsync()
    {
        return await appDbContext.SaveChangesAsync();
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogCritical($"{e.Message}");
            return 0;
        }
    }

    public void Dispose()
    {
        appDbContext.Dispose();
    }

    public IDbContextRepository<T> GetRepository<T>() where T : Entity
    {
        if (_repos.ContainsKey(typeof(T)))
            return (IDbContextRepository<T>)_repos[typeof(T)];
        IDbContextRepository<T> newRepo = new DbContextRepository<T>(appDbContext.Set<T>(), logger);
        _repos.Add(typeof(T), newRepo);
        return newRepo;
    }
}