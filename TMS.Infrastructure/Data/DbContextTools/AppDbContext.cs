namespace TMS.Infrastructure.Data.DbContextTools;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            // "Server=lenovo;Database=TMS;User Id=sa;Password=Rami0000;Encrypt=True;TrustServerCertificate=True;"
            // "Server=DESKTOP-MJIUN3T;Database=TMS;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;"
             "Server=sql.bsite.net\\MSSQL2016;Database=ramialzend_ramialzend;User Id=ramialzend_ramialzend;Password=Rami0000;Encrypt=True;TrustServerCertificate=True;"
            
            );

        return new AppDbContext(optionsBuilder.Options);
    }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<WorkTask> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TaskSubmission> TaskSubmissions { get; set; }
    public DbSet<SubmissionFile> SubmissionFiles { get; set; }

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
            .HasOne(d => d.TeamLeader)
            .WithOne();

        // Configure TaskSubmission relationships
        modelBuilder.Entity<TaskSubmission>()
            .HasOne(ts => ts.WorkTask)
            .WithMany(wt => wt.Submissions)
            .HasForeignKey(ts => ts.WorkTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskSubmission>()
            .HasOne(ts => ts.SubmittedBy)
            .WithMany(e => e.TaskSubmissions)
            .HasForeignKey(ts => ts.SubmittedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure SubmissionFile relationships
        modelBuilder.Entity<SubmissionFile>()
            .HasOne(sf => sf.TaskSubmission)
            .WithMany(ts => ts.Files)
            .HasForeignKey(sf => sf.TaskSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);


        // modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        // modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        // modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        // modelBuilder.ApplyConfiguration(new RoleConfiguration());
        // modelBuilder.ApplyConfiguration(new UserConfiguration());
        // modelBuilder.ApplyConfiguration(new WorkTaskConfiguration());
        
        
        base.OnModelCreating(modelBuilder);
    }
}