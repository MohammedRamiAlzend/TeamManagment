using Microsoft.Extensions.DependencyInjection;

namespace TMS.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDependencyInjection(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection));
        return services;
    }
}