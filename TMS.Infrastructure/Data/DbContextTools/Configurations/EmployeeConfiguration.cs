namespace TMS.Infrastructure.Data.DbContextTools.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        AutoIncludeConfiguration(builder);
        // RelationsConfiguration(builder);
    }

    private static void AutoIncludeConfiguration(EntityTypeBuilder<Employee> builder)
    {
        builder.Navigation(x=>x.User).AutoInclude();
        builder.Navigation(x => x.Departments).AutoInclude();
        // builder.Navigation(x =>x.Departments ).AutoInclude();
    }
    private static void RelationsConfiguration(EntityTypeBuilder<Employee> builder)
    {
        // builder.HasMany(e => e.Departments).WithMany(d => d.Employees);
        // Configure the many-to-many relationship between Employee and Department
        builder.HasMany(e => e.Departments)
            .WithMany(d => d.Employees)
            .UsingEntity(j => j.ToTable("EmployeeDepartments"));

    }
    
}

