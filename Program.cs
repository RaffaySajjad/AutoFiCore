using Microsoft.EntityFrameworkCore;
using AutoFiCore.Data;
using AutoFiCore.Middleware;
using AutoFiCore.Models;
using AutoFiCore.Services;
using Polly;
using Polly.Retry;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var MyAllowSpecificOrigins = "_MyAllowSubdomainPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
    policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure database settings
var databaseSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>()
    ?? throw new InvalidOperationException("Database settings are not configured properly.");
builder.Services.AddSingleton(databaseSettings);

// Configure API settings
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>()
    ?? new ApiSettings { UseMockApi = false };
builder.Services.AddSingleton(apiSettings);

// Configure Entity Framework Core logging and DbContext
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var databaseSettings = serviceProvider.GetRequiredService<DatabaseSettings>();

    options.UseNpgsql(databaseSettings.ConnectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(databaseSettings.CommandTimeoutSeconds);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: databaseSettings.MaxRetryCount,
            maxRetryDelay: TimeSpan.FromSeconds(databaseSettings.RetryDelaySeconds),
            errorCodesToAdd: null);
    });

    // Only log SQL queries in development
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors();
    }
});

// Configure retry policy for database operations
var retryPolicy = Policy
    .Handle<DbUpdateException>()
    .Or<DbUpdateConcurrencyException>()
    .WaitAndRetryAsync(
        databaseSettings.MaxRetryCount,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            // Log retry attempt
            Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
        });

builder.Services.AddSingleton<IAsyncPolicy>(retryPolicy);

// Register repository based on configuration
if (apiSettings.UseMockApi)
{
    builder.Services.AddScoped<IVehicleRepository, MockVehicleRepository>();
}
else
{
    builder.Services.AddScoped<IVehicleRepository, DbVehicleRepository>();
    builder.Services.AddScoped<IContactInfoRepository, DbContactInfoRepository>();
    builder.Services.AddScoped<IUserRepository, DbUserRepository>();
}

// Register user service
builder.Services.AddScoped<IUserService, UserService>();

// Register vehicle service
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Register contact info service
builder.Services.AddScoped<IContactInfoService, ContactInfoService>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

// Use CORS middleware
app.UseCors(MyAllowSpecificOrigins);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();