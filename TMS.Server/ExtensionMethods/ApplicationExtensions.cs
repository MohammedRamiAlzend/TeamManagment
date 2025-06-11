namespace TMS.Server;

public  static class ApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
        var jsonFilePath = Path.Combine(projectRoot, ConfigHelper.PermissionsRolesFileName);
        try
        {
            DataSynchronizer.Synchronize(jsonFilePath);
        }
        catch (Exception ex)
        {
        }

        using (var scope = app.Services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<SeederRunner>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>(); 

            try
            {
                await runner.RunSeedersAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during database seeding.");
            }
        }
    }

}