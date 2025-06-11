namespace TMS.Infrastructure.DataSeeder.Interfaces;

public interface IDataSeeder
{
    EnvironmentEnum Environment { get; }
    int Priority { get; }
    Task<DbRequest> SeedAsync(AppDbContext context);
}

public enum EnvironmentEnum
{
    Development,
    Production,
    All
}