namespace TMS.Infrastructure.Data.DbContextTools.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        AutoIncludeConfiguration(builder);
        // RelationsConfiguration(builder);
    }
    private static void AutoIncludeConfiguration(EntityTypeBuilder<Role> builder)
    {
        builder.Navigation(x=>x.Permissions).AutoInclude();
    }
    private static void RelationsConfiguration(EntityTypeBuilder<Role> builder)
    {
        builder.HasMany(r => r.Permissions).WithMany(p => p.Roles);
    }
}