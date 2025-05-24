using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data.DbContextTools;
using TMS.Infrastructure.Repositories;
using TMS.Infrastructure.Services;

namespace TMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDi(this IServiceCollection services , string connectionString , ConfigurationManager configuration)
    {

        services.AddDbContext<AppDbContext>(
            opt =>
            {
                opt.UseSqlServer(connectionString);
            });
        services.AddScoped<IEntityCommiter, EntityCommiter>()
                .AddScoped(typeof(IDbContextRepository<>), typeof(DbContextRepository<>));

        services.AddScoped<IAuthService,AuthService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("AppSettings:Issuer"),
            
                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("AppSettings:Audience"),
            
                    ValidateLifetime = true,
            
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!))
                };
            });
        return services;
    }
}
