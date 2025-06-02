using AutoFiCore.Data;
using AutoFiCore.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AutoFiCore.Services;

public interface IVehicleService
{
    Task<VehicleListResult> GetAllVehiclesByStatusAsync(int pageView, int offset, string? status = null);
    Task<VehicleListResult> GetVehiclesByMakeAsync(int pageView, int offset, string make);
    Task<VehicleListResult> GetVehiclesByModelAsync(int pageView, int offset, string make);
    Task<List<string>> GetDistinctColorsAsync();
    Task<VehicleListResult> SearchVehiclesAsync(
        int pageView,
        int offset,
        string? make,
        string? model,
        decimal? startPrice,
        decimal? endPrice,
        int? mileage = null,
        int? startYear = null,
        int? endYear = null,
        string? sortOrder = null,
        string? gearbox = null,
        string? selectedColors = null,
        string? status = null
        );
    Task<List<VehicleModelJSON>> GetAllCarFeaturesAsync();
    VehicleModelJSON? GetCarFeature(List<VehicleModelJSON>? carFeatures, string make, string model);
    Task<List<string>> GetAllVehiclesMakesAsync();
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
    public VehicleModelJSON? GetCarFeature(List<VehicleModelJSON>? carFeatures, string make, string model)
    {
        try
        {
             if (carFeatures == null || carFeatures.Count == 0)
                return null;

            return _repository.GetCarFeature(carFeatures, make, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Car Feature");
            throw;
        }
    }


    public async Task<List<VehicleModelJSON>> GetAllCarFeaturesAsync()
    {
        try
        {
            return await _repository.GetAllCarFeaturesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Car Features");
            throw;
        }
    }

    public async Task<List<string>> GetDistinctColorsAsync()
    {
        try
        {
            return await _repository.GetDistinctColorsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Car Features");
            throw;
        }
    }
    public async Task<VehicleListResult> GetAllVehiclesByStatusAsync(int pageView, int offset, string? status = null)
    {
        try
        {
            return await _repository.GetAllVehiclesByStatusAsync(pageView, offset, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            throw;
        }
    }

    public async Task<List<string>> GetAllVehiclesMakesAsync()
    {
        try
        {
            return await _repository.GetAllVehicleMakes();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle makes");
            throw;
        }
    }

    public async Task<VehicleListResult> GetVehiclesByMakeAsync(int pageView, int offset, string make)
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

    public async Task<VehicleListResult> SearchVehiclesAsync(
        int pageView, 
        int offset,
        string? make, 
        string? model, 
        decimal? startPrice, 
        decimal? endPrice, 
        int? mileage = null,
        int? startYear = null,
        int? endYear = null,
        string? sortOrder = null,
        string? gearbox = null,
        string? selectedColors = null,
        string? status = null
        )
    {
        try
        {
            return await _repository.SearchVehiclesAsync(pageView, offset, make, model, startPrice, endPrice, mileage, startYear, endYear, sortOrder, gearbox, selectedColors, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles by make {Make}", make);
            throw;
        }
    }

    public async Task<VehicleListResult> GetVehiclesByModelAsync(int pageView, int offset, string model)
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
            return await _repository.GetVehicleByVinAsync(vin);
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