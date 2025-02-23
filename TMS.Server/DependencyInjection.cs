using TMS.Core;
using TMS.Core.Queries;
using TMS.Infrastructure;
using TMS.Application;
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
