using System.Text.Json;
using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFiCore.Data;

public class MockVehicleRepository : IVehicleRepository
{
    private readonly string _dataFilePath;
    private readonly ILogger<MockVehicleRepository> _logger;
    private List<Vehicle> _vehicles = new();

    public MockVehicleRepository(IConfiguration configuration, IWebHostEnvironment environment, 
        ILogger<MockVehicleRepository> logger)
    {
        _logger = logger;
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Data", "MockVehicles.json");
        _logger.LogInformation($"Data file path: {_dataFilePath}");
        LoadVehiclesFromFile();
    }

    private void LoadVehiclesFromFile()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                _logger.LogInformation($"Loading vehicles from file: {_dataFilePath}");
                var json = File.ReadAllText(_dataFilePath);
                
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };
                
                _vehicles = JsonSerializer.Deserialize<List<Vehicle>>(json, options) ?? new List<Vehicle>();
                _logger.LogInformation($"Loaded {_vehicles.Count} vehicles from file");
                
                // Log the first vehicle as a sample
                if (_vehicles.Count > 0)
                {
                    var firstVehicle = _vehicles[0];
                    _logger.LogInformation($"Sample vehicle - Id: {firstVehicle.Id}, Make: {firstVehicle.Make}, Model: {firstVehicle.Model}");
                }
            }
            else
            {
                _logger.LogWarning($"Mock vehicles data file not found at path: {_dataFilePath}");
                _vehicles = new List<Vehicle>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading vehicles from file: {ex.Message}");
            _vehicles = new List<Vehicle>();
        }
    }

    private async Task SaveVehiclesToFile()
    {
        try
        {
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            
            var json = JsonSerializer.Serialize(_vehicles, options);
            await File.WriteAllTextAsync(_dataFilePath, json);
            _logger.LogInformation($"Saved {_vehicles.Count} vehicles to file");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving vehicles to file: {ex.Message}");
        }
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(int pageView, int offset)
    {
        // Reload from file to get fresh data
        LoadVehiclesFromFile();
        return await Task.FromResult(_vehicles);
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByMakeAsync(int pageView, int offset, string make)
    {
        LoadVehiclesFromFile();

        var filteredVehicles = _vehicles.OrderBy(v=>v.Id)
            .Where(v => v.Make.ToLower() == make.ToLower())
            .Skip(offset)
            .Take(pageView)
            .ToList();


        return await Task.FromResult(filteredVehicles);
    }

    public async Task<List<string>> GetAllVehicleMakes()
    {
        LoadVehiclesFromFile();
        var makes = _vehicles
            .Select(v => v.Make)
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        return await Task.FromResult(makes);
    }
    public async Task<IEnumerable<Vehicle>> GetVehiclesByModelAsync(int pageView, int offset, string model)
    {
        LoadVehiclesFromFile();
        var filteredVehicles = _vehicles.OrderBy(v=>v.Id)
             .Where(v => v.Model.ToLower() == model.ToLower())
             .Skip(offset)
             .Take(pageView)
             .ToList();
        return await Task.FromResult(filteredVehicles);
    }

   
    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        return await Task.FromResult(_vehicles.FirstOrDefault(v => v.Id == id));
    }

    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        // Generate a new ID for the vehicle
        vehicle.Id = _vehicles.Any() ? _vehicles.Max(v => v.Id) + 1 : 1;
        _vehicles.Add(vehicle);
        await SaveVehiclesToFile();
        return vehicle;
    }

    public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
    {
        var index = _vehicles.FindIndex(v => v.Id == vehicle.Id);
        if (index == -1)
            return false;

        _vehicles[index] = vehicle;
        await SaveVehiclesToFile();
        return true;
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
        if (vehicle == null)
            return false;

        _vehicles.Remove(vehicle);
        await SaveVehiclesToFile();
        return true;
    }
} 