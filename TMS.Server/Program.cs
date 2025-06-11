using Newtonsoft.Json;
using TMS.Server.Helpers;
using TMS.Server.PermissionsAndRolesConfig;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
builder.Services.AddOpenApi();
builder.Services.AddHostedService<DatabaseSyncService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
ArgumentNullException.ThrowIfNull(connectionString);
builder.Services.AddAppDependencyInjection(connectionString, builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
var jsonFilePath = Path.Combine(projectRoot, ConfigHelper.PermissionsRolesFileName);
try
{
    DataSynchronizer.Synchronize(jsonFilePath);
}
catch (Exception ex)
{
}

using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<SeederRunner>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await runner.RunSeedersAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();