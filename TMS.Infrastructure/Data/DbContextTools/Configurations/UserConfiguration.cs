using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TMS.Infrastructure.Data.DbContextTools.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        AutoIncludeConfiguration(builder);
        // RelationsConfiguration(builder);
    }
    private static void AutoIncludeConfiguration(EntityTypeBuilder<User> builder)
    {
        builder.Navigation(x=>x.Roles).AutoInclude();
    }
    private static void RelationsConfiguration(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.Roles).WithMany(r => r.Users);
    }
}