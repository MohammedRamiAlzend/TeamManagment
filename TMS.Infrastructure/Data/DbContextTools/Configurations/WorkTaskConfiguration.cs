using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TMS.Infrastructure.Data.DbContextTools.Configurations;

public class WorkTaskConfiguration:IEntityTypeConfiguration<WorkTask>
{
    public void Configure(EntityTypeBuilder<WorkTask> builder)
    {
        AutoIncludeConfiguration(builder);
        // RelationsConfiguration(builder);
    }
    private static void AutoIncludeConfiguration(EntityTypeBuilder<WorkTask> builder)
    {
        builder.Navigation(x=>x.Projects).AutoInclude();
    }
    private static void RelationsConfiguration(EntityTypeBuilder<WorkTask> builder)
    {
        builder
            .HasMany(t => t.Projects)
            .WithMany(p => p.Tasks);
        builder
            .HasOne(t => t.CreatedBy)
            .WithMany(e => e.CreatedTasks)
            .HasForeignKey(t => t.CreatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(t => t.AssignedTo)
            .WithMany(e => e.AssignedTasks)
            .HasForeignKey(t => t.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}