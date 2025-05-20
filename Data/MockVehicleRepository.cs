using System.Text.Json;
using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AutoFiCore.Data;

public class MockVehicleRepository : IVehicleRepository
{
    private readonly string _dataFilePath;
    private readonly ILogger<MockVehicleRepository> _logger;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private List<Vehicle> _vehicles = new();

    public MockVehicleRepository(IConfiguration configuration, IWebHostEnvironment environment, 
        ILogger<MockVehicleRepository> logger, IWebHostEnvironment hostingEnvironment)
    {
        _logger = logger;
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Data", "MockVehicles.json");
        _logger.LogInformation($"Data file path: {_dataFilePath}");
        LoadVehiclesFromFile();
        _hostingEnvironment = hostingEnvironment;
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
    public VehicleModelJSON? GetCarFeature(List<VehicleModelJSON>? carFeatures, string make, string model)
    {
        if (carFeatures == null)
            return null;

        return carFeatures.FirstOrDefault(c => string.Equals(c.Make, make, StringComparison.OrdinalIgnoreCase) && string.Equals(c.Model, model, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<VehicleModelJSON>> GetAllCarFeaturesAsync()
    {
        try
        {
            var rootPath = _hostingEnvironment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, "Data", "car-features.json");

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("Data file not found at path: {Path}", fullPath);
                return new List<VehicleModelJSON>();
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(fullPath);

            if (string.IsNullOrWhiteSpace(jsonData))
            {
                _logger.LogWarning("Data file is empty: {Path}", fullPath);
                return new List<VehicleModelJSON>();
            }

            var carFeatures = JsonConvert.DeserializeObject<List<VehicleModelJSON>>(jsonData);

            return carFeatures ?? new List<VehicleModelJSON>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving car features");
            throw;
        }
    }
    public async Task<VehicleListResult> GetAllVehiclesAsync(int pageView, int offset)
    {
        var totalVehicles = _vehicles.Count;

        var vehicles = _vehicles
        .OrderBy(v => v.Id)
        .Skip(offset)
        .Take(pageView)
        .ToList();

        var result = new VehicleListResult
        {
            Vehicles = vehicles,
            TotalCount = totalVehicles,
        };
        return await Task.FromResult( result );
    }

    public async Task<VehicleListResult> SearchVehiclesAsync(int pageView, int offset, string? make = null, string? model = null, decimal? startPrice = null, decimal? endPrice = null, int? milage = null, string? sortOrder = null)
    {
        try
        {
            var query = _vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(make) && make != "Any Makes")
            {
                query = query.Where(v => v.Make == make);
            }

            if (!string.IsNullOrWhiteSpace(model) && model != "Any Models")
            {
                query = query.Where(v => v.Model == model);
            }


            if (startPrice.HasValue)
            {
                query = query.Where(v => v.Price >= startPrice.Value);
            }

            if (endPrice.HasValue)
            {
                query = query.Where(v => v.Price <= endPrice.Value);
            }

            if (milage.HasValue)
            {
                query = query.Where(v => v.Mileage <= milage.Value);
            }
            int totalCount = query.Count();

            query = sortOrder switch
            {
                "price_asc" => query.OrderBy(v => v.Price),
                "price_desc" => query.OrderByDescending(v => v.Price),
                "mileage_asc" => query.OrderBy(v => v.Mileage),
                "mileage_desc" => query.OrderByDescending(v => v.Mileage),
                "name_asc" => query.OrderBy(v => v.Make).ThenBy(v => v.Model),
                "name_desc" => query.OrderByDescending(v => v.Make).ThenByDescending(v => v.Model),
                _ => query.OrderBy(v => v.Id)
            };


            var vehicles = query
                .Skip(offset)
                .Take(pageView)
                .ToList();

            return await Task.FromResult(new VehicleListResult
            {
                Vehicles = vehicles,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vehicles");
            throw;
        }
    }

    public async Task<VehicleListResult> GetVehiclesByMakeAsync(int pageView, int offset, string make)
    {
       
        var query = _vehicles.Where(v => v.Make.ToLower() == make.ToLower());
        
        var vehicles = query.OrderBy(v=>v.Id)
            .Skip(offset)
            .Take(pageView)
            .ToList();

        var result = new VehicleListResult
        {
            Vehicles = vehicles,
            TotalCount = vehicles.Count
        };

        return await Task.FromResult(result);
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
    public async Task<VehicleListResult> GetVehiclesByModelAsync(int pageView, int offset, string model)
    {
        var query = _vehicles.Where(v => v.Model.ToLower() == model.ToLower());

        var vehicles = query.OrderBy(v => v.Id)
            .Skip(offset)
            .Take(pageView)
            .ToList();

        var result = new VehicleListResult
        {
            Vehicles = vehicles,
            TotalCount = vehicles.Count
        };

        return await Task.FromResult(result);
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