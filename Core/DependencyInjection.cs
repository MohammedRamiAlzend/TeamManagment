using Microsoft.Extensions.DependencyInjection;

namespace TMS.Core;
public static class DependencyInjection
{
    public static IServiceCollection AddCoreDI(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection));
        return services;
    }
}
