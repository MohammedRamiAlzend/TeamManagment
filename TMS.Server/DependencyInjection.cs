using TMS.Application;
using TMS.Application.Extensions;
using TMS.Infrastructure;

namespace TMS.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddAppDependencyInjection(this IServiceCollection services,
        string connectionString, ConfigurationManager configuration)
    {
        services.AddApplicationDependencyInjection()
            .AddInfrastructureDependencyInjection(connectionString, configuration)
            .AddCoreDependencyInjection();
        return services;
    }
}