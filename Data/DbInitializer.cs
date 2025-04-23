using System.Text.Json;
using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFiCore.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, IWebHostEnvironment env)
    {
        using var scope = serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        
        // Skip if using mock API
        if (configuration.GetValue<bool>("UseMockApi"))
        {
            return;
        }

        var context = scope.ServiceProvider.GetRequiredService<AutoFiDbContext>();
        
        // Create database if it doesn't exist
        await context.Database.MigrateAsync();
        
        // Check if database already has vehicles
        if (await context.Vehicles.AnyAsync())
        {
            return; // Database already seeded
        }
        
        // Load mock data from file to seed the database
        var dataFilePath = Path.Combine(env.ContentRootPath, "Data", "MockVehicles.json");
        if (File.Exists(dataFilePath))
        {
            var json = await File.ReadAllTextAsync(dataFilePath);
            var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(json);
            
            if (vehicles != null)
            {
                await context.Vehicles.AddRangeAsync(vehicles);
                await context.SaveChangesAsync();
            }
        }
    }
} 