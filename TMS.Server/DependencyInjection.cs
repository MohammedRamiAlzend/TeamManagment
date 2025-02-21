using TMS.Application;
using TMS.Core;
using TMS.Infrastructure;
namespace TMS.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddAppDI(this IServiceCollection services,string connectionString)
    {
        services.AddApplicationDI()
                .AddInfrastructureDI(connectionString)
                .AddCoreDI();
        return services;
    }
}
