using AutoFiCore.Data;
using AutoFiCore.Models;
using Microsoft.Extensions.Logging;

namespace AutoFiCore.Services;

public interface IVehicleService
{
    Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(int pageView, int offset);
    Task<IEnumerable<Vehicle>> GetVehiclesByMakeAsync(int pageView, int offset, string make);
    Task<IEnumerable<Vehicle>> GetVehiclesByModelAsync(int pageView, int offset, string make);
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<Vehicle?> GetVehicleByVinAsync(string vin);
    Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
    Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
    Task<bool> DeleteVehicleAsync(int id);
}

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repository;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(IVehicleRepository repository, ILogger<VehicleService> logger)
    {
        _repository = repository;
        _logger = logger;
    } 
    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(int pageView, int offset)
    {
        try
        {
            return await _repository.GetAllVehiclesAsync(pageView, offset);
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
            return await _repository.GetVehiclesByMakeAsync(pageView, offset, make);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles by make {Make}", make);
            throw;
        }
    }
    public async Task<IEnumerable<Vehicle>> GetVehiclesByModelAsync(int pageView, int offset, string model)
    {
        try
        {
            return await _repository.GetVehiclesByModelAsync(pageView, offset, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles by model {Model}", model);
            throw;
        }
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        try
        {
            return await _repository.GetVehicleByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle with ID {Id}", id);
            throw;
        }
    }

    public async Task<Vehicle?> GetVehicleByVinAsync(string vin)
    {
        try
        {
            var vehicles = await _repository.GetAllVehiclesAsync(10, 0);
            return vehicles.FirstOrDefault(v => v.Vin == vin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle with VIN {Vin}", vin);
            throw;
        }
    }

    public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
    {
        try
        {
            return await _repository.AddVehicleAsync(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle with VIN {Vin}", vehicle.Vin);
            throw;
        }
    }

    public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
    {
        try
        {
            var success = await _repository.UpdateVehicleAsync(vehicle);
            if (!success)
            {
                throw new InvalidOperationException($"Vehicle with ID {vehicle.Id} not found");
            }
            return vehicle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle with ID {Id}", vehicle.Id);
            throw;
        }
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        try
        {
            return await _repository.DeleteVehicleAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle with ID {Id}", id);
            throw;
        }
    }
} 