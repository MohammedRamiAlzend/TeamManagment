using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.DbContextTools;
  public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-MJIUN3T;Database=TMS;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;"); 

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
    public  DbSet<Permission> Permissions { get; set; }
    public  DbSet<Role> Roles { get; set; }
    public  DbSet<WorkTask> Tasks { get; set; }
    public  DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Departments)
            .WithMany(d => d.Employees);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.TeamMembers)
            .WithMany(e => e.Projects);

        modelBuilder.Entity<WorkTask>()
            .HasMany(t => t.Projects)
            .WithMany(p => p.Tasks);
        
        
        modelBuilder.Entity<WorkTask>()
            .HasOne(t => t.CreatedBy)
            .WithMany(e => e.CreatedTasks)
            .HasForeignKey(t => t.CreatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkTask>()
            .HasOne(t => t.AssignedTo)
            .WithMany(e => e.AssignedTasks)
            .HasForeignKey(t => t.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>()
            .HasOne(d=>d.TeamLeader)
            .WithOne();
        
        base.OnModelCreating(modelBuilder);
    }
}