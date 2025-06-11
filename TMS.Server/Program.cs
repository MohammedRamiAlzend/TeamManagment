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

await app.InitializeDatabaseAsync();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();