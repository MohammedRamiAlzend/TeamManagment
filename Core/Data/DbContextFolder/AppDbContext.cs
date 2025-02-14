using Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Core.Data.DbContextFolder
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Claim> Claims{ get; set; }
        public DbSet<Role> Roles{ get; set; }
        public DbSet<WorkTask> Tasks{ get; set; }
        public DbSet<TaskAssignment> TaskAssignments{ get; set; }
        public DbSet<Point> Points{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity => {
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
}