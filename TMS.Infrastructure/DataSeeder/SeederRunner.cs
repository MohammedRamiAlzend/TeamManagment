namespace TMS.Infrastructure.DataSeeder;

public class SeederRunner(IServiceProvider serviceProvider, ILogger<SeederRunner> _logger, DataSeederFactory factory)
{
    public async Task RunSeedersAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var seeders = factory.GetSeeders();

            foreach (var seeder in seeders)
            {
                _logger.LogInformation("Running seeder: {SeederName}", seeder.GetType().Name);
                var result = await seeder.SeedAsync(context);
                ;
            }

            // await transaction.CommitAsync();
            _logger.LogInformation("Seeding completed successfully.");
        }
        catch (Exception ex)
        {
            // await transaction.RollbackAsync();
            _logger.LogError(ex, "Seeding failed. Transaction rolled back.");
        }
    }
}