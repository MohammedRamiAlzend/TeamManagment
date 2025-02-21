using System;
using Microsoft.Extensions.DependencyInjection;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data.DbContextTools;
using TMS.Infrastructure.Repositories;

namespace TMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDI(this IServiceCollection services , string connectionString)
    {

        services.AddDbContext<AppDbContext>(
            opt =>
            {
                opt.UseSqlServer(connectionString);
            });
        services.AddScoped<IEntityCommiter, EntityCommiter>()
                .AddScoped(typeof(IDbContextRepository<>), typeof(DbContextRepository<>));

        return services;
    }
}
