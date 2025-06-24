using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Contract.Entities;

namespace TMS.Infrastructure.Data.Configuration;

public class TaskSubmissionConfiguration : IEntityTypeConfiguration<TaskSubmission>
{
    public void Configure(EntityTypeBuilder<TaskSubmission> builder)
    {
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Description)
            .HasMaxLength(4000);

        builder.Property(ts => ts.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.FeedbackComments)
            .HasMaxLength(4000);

        // Relationship with WorkTask
        builder.HasOne(ts => ts.WorkTask)
            .WithMany(wt => wt.Submissions)
            .HasForeignKey(ts => ts.WorkTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship with Employee
        builder.HasOne(ts => ts.SubmittedBy)
            .WithMany(e => e.TaskSubmissions)
            .HasForeignKey(ts => ts.SubmittedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
