using Microsoft.EntityFrameworkCore;
using AutoFiCore.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Microsoft.AspNetCore.Mvc;

namespace AutoFiCore.Data;

public class DbVehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DbVehicleRepository> _logger;

    public DbVehicleRepository(ApplicationDbContext dbContext, ILogger<DbVehicleRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
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

    public async Task<VehicleListResult> SearchVehiclesAsync(int pageView, int offset, string? make = null, string? model = null, decimal? startPrice = null, decimal? endPrice = null)
    {
        try
        {
            var query = _dbContext.Vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(make))
            {
                query = query.Where(v => v.Make == make);
            }

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

            int totalCount = await query.CountAsync();

            var vehicles = await query
                .OrderBy(v => v.Id)
                .Skip(offset)
                .Take(pageView)
                .ToListAsync();

            return new VehicleListResult
            {
                Vehicles = vehicles,
                TotalCount = totalCount,
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