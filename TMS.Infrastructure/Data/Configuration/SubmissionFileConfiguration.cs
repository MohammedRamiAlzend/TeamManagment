using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Contract.Entities;

namespace TMS.Infrastructure.Data.Configuration;

public class SubmissionFileConfiguration : IEntityTypeConfiguration<SubmissionFile>
{
    public void Configure(EntityTypeBuilder<SubmissionFile> builder)
    {
        builder.HasKey(sf => sf.Id);

        builder.Property(sf => sf.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(sf => sf.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(sf => sf.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(sf => sf.FileExtension)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sf => sf.ContentType)
            .IsRequired()
            .HasMaxLength(255);

        // Relationship with TaskSubmission
        builder.HasOne(sf => sf.TaskSubmission)
            .WithMany(ts => ts.Files)
            .HasForeignKey(sf => sf.TaskSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
