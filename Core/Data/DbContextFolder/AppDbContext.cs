using Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Core.Data.DbContextFolder
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public required DbSet<Employee> Employees { get; set; }
        public required DbSet<Department> Departments { get; set; }
        public required DbSet<Claim> Claims{ get; set; }
        public required DbSet<Role> Roles{ get; set; }
        public required DbSet<WorkTask> Tasks{ get; set; }
        public required DbSet<TaskAssignment> TaskAssignments{ get; set; }
        public required DbSet<Point> Points{ get; set; }
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