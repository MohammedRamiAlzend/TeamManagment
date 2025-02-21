using TMS.Server;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
ArgumentNullException.ThrowIfNull(connectionString);
builder.Services.AddAppDI(connectionString);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
