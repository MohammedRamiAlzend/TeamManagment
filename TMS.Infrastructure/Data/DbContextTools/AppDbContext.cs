using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.DbContextTools;
  public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=lenovo;Database=TMS;User Id=sa;Password=Rami0000;Encrypt=True;TrustServerCertificate=True;"); // Replace with your actual connection string

            return new AppDbContext(optionsBuilder.Options);
        }
    }
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public  DbSet<Employee> Employees { get; set; }
    public  DbSet<Department> Departments { get; set; }
    public  DbSet<Claim> Claims { get; set; }
    public  DbSet<Role> Roles { get; set; }
    public  DbSet<WorkTask> Tasks { get; set; }
    public  DbSet<TaskAssignment> TaskAssignments { get; set; }
    public  DbSet<Point> Points { get; set; }
    public  DbSet<Project> Projects { get; set; }
    public  DbSet<ProjectTeamMember> ProjectTeamMembers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasMany(x => x.AssignedToEmployeeTasks)
            .WithOne(x => x.AssignedToEmployee)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(x => x.AssignedByEmployeeTasks)
            .WithOne(x => x.AssignedByEmployee)
            .OnDelete(DeleteBehavior.NoAction);
        });
        base.OnModelCreating(modelBuilder);
    }
}