using Microsoft.EntityFrameworkCore;
using AutoFiCore.Data;
using AutoFiCore.Middleware;

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

// Add our execution time logging middleware
app.UseRequestExecutionTimeLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("OpenAPI Swagger UI available at: /swagger");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
