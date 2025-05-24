using TMS.Core;
using TMS.Infrastructure;
using TMS.Application;
namespace TMS.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddAppDi(this IServiceCollection services,string connectionString,ConfigurationManager configuration)
    {
        services.AddApplicationDependencyInjection()
                .AddInfrastructureDi(connectionString,configuration)
                .AddCoreDi();
        return services;
    }
}
