using TMS.Infrastructure.AppConfigurations;
using TMS.Infrastructure.DataSeeder;
using TMS.Server;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// ✅ إضافة سياسة CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddOpenApi();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
ArgumentNullException.ThrowIfNull(connectionString);

builder.Services.AddAppDi(connectionString, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

PermissionSettings.LoadPermissionsConfig();

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

app.UseCors("AllowReactApp"); // ✅ تفعيل سياسة CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
