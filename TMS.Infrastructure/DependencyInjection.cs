namespace TMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencyInjection(this IServiceCollection services,
        string connectionString, ConfigurationManager configuration)
    {
        services.AddMemoryCache();

        services.AddDbContext<AppDbContext>(opt => { opt.UseSqlServer(connectionString); });
        services.AddScoped<IEntityCommiter, EntityCommiter>()
            .AddScoped(typeof(IDbContextRepository<>), typeof(DbContextRepository<>));

        services.AddScoped<IAuthService, AuthService>();
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

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, LogicalPermissionHandler>();


        services.AddSingleton<DataSeederFactory>();
        services.AddScoped<SeederRunner>();

        return services;
    }
}