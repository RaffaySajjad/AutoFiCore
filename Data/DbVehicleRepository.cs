using Microsoft.EntityFrameworkCore;
using AutoFiCore.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AutoFiCore.Data;

public class DbVehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DbVehicleRepository> _logger;
    private readonly IWebHostEnvironment _hostingEnvironment;


    public DbVehicleRepository(ApplicationDbContext dbContext, ILogger<DbVehicleRepository> logger, IWebHostEnvironment hostingEnvironment)
    {
        _dbContext = dbContext;
        _logger = logger;
        _hostingEnvironment = hostingEnvironment;
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
        try 
        {
            var totalVehicles = await _dbContext.Vehicles.CountAsync();
            var result = await _dbContext.Vehicles.OrderBy(v=>v.Id).Skip(offset).Take(pageView).ToListAsync();
            return new VehicleListResult
            {
                Vehicles = result,
                TotalCount = totalVehicles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            throw;
        }
    }

    public async Task<List<string>> GetAllVehicleMakes()
    {
        var makes = await _dbContext.Vehicles
            .Select(v => v.Make)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();

        return await Task.FromResult(makes);
    }

    public async Task<VehicleListResult> GetVehiclesByMakeAsync(int pageView, int offset, string make)
    {
        try
        {
            var query = _dbContext.Vehicles.Where(v => v.Make.ToLower() == make.ToLower());
            var totalVehicles = await query.CountAsync();


            var result = await query.OrderBy(v=>v.Id)
                .Skip(offset)
                .Take(pageView)
                .ToListAsync();
            return new VehicleListResult
            {
                Vehicles = result,
                TotalCount = totalVehicles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving vehicles by make: {make}");
            throw;
        }
    }

    public async Task<VehicleListResult> GetVehiclesByModelAsync(int pageView, int offset, string model)
    {
        try
        {
            var query = _dbContext.Vehicles.Where(v => v.Model.ToLower() == model.ToLower());
            var totalVehicles = await query.CountAsync();
            
            var result = await query.OrderBy(v => v.Id)
                .Skip(offset)
                .Take(pageView)
                .ToListAsync();

            return new VehicleListResult
            {
                Vehicles = result,
                TotalCount = totalVehicles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving vehicles by model: {model}");
            throw;
        }
    }

    public async Task<VehicleListResult> SearchVehiclesAsync(
    int pageView,
    int offset,
    string? make = null,
    string? model = null,
    decimal? startPrice = null,
    decimal? endPrice = null,
    int? mileage = null,
    string? sortOrder = null)
    {
        try
        {
            var query = _dbContext.Vehicles.AsQueryable();

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

            if (mileage.HasValue)
            {
                query = query.Where(v => v.Mileage <= mileage.Value);
            }

            int totalCount = await query.CountAsync();

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

            var vehicles = await query
                .Skip(offset)
                .Take(pageView)
                .ToListAsync();

            return new VehicleListResult
            {
                Vehicles = vehicles,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vehicles");
            throw;
        }
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        try
        {
            return await _dbContext.Vehicles.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle with ID {Id}", id);
            throw;
        }
    }

    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        try
        {
            _dbContext.Vehicles.Add(vehicle);
            await _dbContext.SaveChangesAsync();
            return vehicle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding vehicle with VIN {Vin}", vehicle.Vin);
            throw;
        }
    }

    public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
    {
        try
        {
            _dbContext.Entry(vehicle).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(vehicle.Id))
                return false;
            
            throw;
        }
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        try
        {
            var vehicle = await _dbContext.Vehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            _dbContext.Vehicles.Remove(vehicle);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle with ID {Id}", id);
            throw;
        }
    }

    private bool VehicleExists(int id)
    {
        return _dbContext.Vehicles.Any(v => v.Id == id);
    }
} 