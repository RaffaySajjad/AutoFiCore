using Microsoft.EntityFrameworkCore;
using AutoFiCore.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure database context
if (builder.Configuration.GetValue<bool>("UseMockApi"))
{
    // Log "Using mock API"
    builder.Logging.AddConsole();
    Console.WriteLine("Using mock API");
    // Use mock repository when MOCK_API is true
    builder.Services.AddScoped<IVehicleRepository, MockVehicleRepository>();
}
else
{
    Console.WriteLine("Using database");
    // Use database with Entity Framework when MOCK_API is false
    builder.Services.AddDbContext<AutoFiDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IVehicleRepository, DbVehicleRepository>();
}

var app = builder.Build();

// Initialize database
await DbInitializer.InitializeAsync(app.Services, app.Environment);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
