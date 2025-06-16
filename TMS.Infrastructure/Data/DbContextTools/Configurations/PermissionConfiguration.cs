using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TMS.Infrastructure.Data.DbContextTools.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        AutoIncludeConfiguration(builder);
        // RelationsConfiguration(builder);
    }

    private static void AutoIncludeConfiguration(EntityTypeBuilder<Permission> builder)
    {
        builder.Navigation(x=>x.Roles).AutoInclude();
    }
    private static void RelationsConfiguration(EntityTypeBuilder<Permission> builder)
    {
        
    }
}