using Microsoft.EntityFrameworkCore;
using AutoFiCore.Models;
using Microsoft.Extensions.Logging;

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

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(int pageView, int offset)
    {
        try 
        {
            var result = await _dbContext.Vehicles.Skip(offset).Take(pageView).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            throw;
        }
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByMakeAsync(int pageView, int offset, string make)
    {
        try
        {
            var result = await _dbContext.Vehicles
                .Where(v => v.Make.ToLower() == make.ToLower())
                .Skip(offset)
                .Take(pageView)
                .ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving vehicles by make: {make}");
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