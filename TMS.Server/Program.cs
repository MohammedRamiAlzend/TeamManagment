var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

// builder.Services.AddOpenApi();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddHostedService<DatabaseSyncService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
ArgumentNullException.ThrowIfNull(connectionString);
builder.Services.AddAppDependencyInjection(connectionString, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy",
        policy =>
        {
            policy
                .AllowAnyOrigin() 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Servers = [];
    });
// }
    app.UseCors("DevelopmentCorsPolicy");

await app.InitializeDatabaseAsync();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


