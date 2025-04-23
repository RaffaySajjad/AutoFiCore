using Microsoft.EntityFrameworkCore;
using AutoFiCore.Models;

namespace AutoFiCore.Data;

public class DbVehicleRepository : IVehicleRepository
{
    private readonly AutoFiDbContext _dbContext;

    public DbVehicleRepository(AutoFiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        return await _dbContext.Vehicles.ToListAsync();
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        return await _dbContext.Vehicles.FindAsync(id);
    }

    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        _dbContext.Vehicles.Add(vehicle);
        await _dbContext.SaveChangesAsync();
        return vehicle;
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
        var vehicle = await _dbContext.Vehicles.FindAsync(id);
        if (vehicle == null)
            return false;

        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private bool VehicleExists(int id)
    {
        return _dbContext.Vehicles.Any(v => v.Id == id);
    }
} 