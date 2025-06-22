namespace TMS.Infrastructure.Data.DbContextTools.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        AutoIncludeConfiguration(builder);
        // RelationsConfiguration(builder);
    }
    private static void AutoIncludeConfiguration(EntityTypeBuilder<Project> builder)
    {
        builder.Navigation(x=>x.TeamMembers).AutoInclude();
        builder.Navigation(x => x.Tasks).AutoInclude();
        builder.Navigation(x => x.Department).AutoInclude();
        
    }
    private static void RelationsConfiguration(EntityTypeBuilder<Project> builder)
    {  
        builder.HasMany(p => p.TeamMembers).WithMany(e => e.Projects);
    }
}