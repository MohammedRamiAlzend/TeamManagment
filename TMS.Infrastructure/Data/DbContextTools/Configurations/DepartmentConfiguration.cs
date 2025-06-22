namespace TMS.Infrastructure.Data.DbContextTools.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        AutoIncludeConfiguration(builder);
        // RelationsConfiguration(builder);
    }
    private static void AutoIncludeConfiguration(EntityTypeBuilder<Department> builder)
    {
        builder.Navigation(x => x.TeamLeader).AutoInclude();
    }
    private static void RelationsConfiguration(EntityTypeBuilder<Department> builder)
    {
        // builder.HasOne(d => d.TeamLeader).WithOne();
        // Configure self-referencing relationship for parent/child departments
        builder.HasOne(d => d.ParentDepartment)
            .WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId);

        // Configure the team leader relationship
        builder.HasOne(d => d.TeamLeader)
            .WithMany() // Employee doesn't have a back-reference collection for led departments
            .HasForeignKey(d => d.TeamLeaderId)
            .OnDelete(DeleteBehavior.SetNull); // Allow null when employee is deleted

        
    }
}