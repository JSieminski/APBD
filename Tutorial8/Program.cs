using Tutorial8.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<ITripsService, TripsService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();