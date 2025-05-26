using Microsoft.AspNetCore.Hosting;
using TMS.Infrastructure.DataSeeder.Interfaces;

namespace TMS.Infrastructure.DataSeeder;

public class DataSeederFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _currentEnvironment;

    public DataSeederFactory(IServiceProvider serviceProvider, IWebHostEnvironment environment)
    {
        _serviceProvider = serviceProvider;
        _currentEnvironment = environment.EnvironmentName;
    }
    public IEnumerable<IDataSeeder> GetSeeders()
    {
        var seeders = typeof(IDataSeeder).Assembly
            .GetTypes()
            .Where(t => typeof(IDataSeeder).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IDataSeeder>()
            .Where(s => s.Environment == EnvironmentEnum.All || s.Environment.ToString() == _currentEnvironment)
            .OrderBy(s => s.Priority)
            .ToList();

        return seeders;
    }
}