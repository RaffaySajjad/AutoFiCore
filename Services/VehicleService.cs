using AutoFiCore.Data;
using AutoFiCore.Dto;
using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Threading.Tasks;

namespace AutoFiCore.Services;

public interface IVehicleService
{
    Task<VehicleListResult> GetAllVehiclesByStatusAsync(int pageView, int offset, string? status = null);
    Task<VehicleListResult> GetVehiclesByMakeAsync(int pageView, int offset, string make);
    Task<VehicleListResult> GetVehiclesByModelAsync(int pageView, int offset, string make);
    Task<List<string>> GetDistinctColorsAsync();
    Task<List<Vehicle>> SearchVehiclesAsync(VehicleFilterDto filters, int pageView, int offset, string? sortOrder = null);
    Task<int> GetTotalCountAsync(VehicleFilterDto filterDto);
    Task<Dictionary<string, int>> GetAvailableColorsCountAsync(VehicleFilterDto filterDto);
    Task<Dictionary<string, int>> GetGearboxCountsAsync(VehicleFilterDto filterDto);
    Task<List<VehicleModelJSON>> GetAllCarFeaturesAsync();
    VehicleModelJSON? GetCarFeature(List<VehicleModelJSON>? carFeatures, string make, string model);
    Task<List<string>> GetAllVehiclesMakesAsync();
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<Vehicle?> GetVehicleByVinAsync(string vin);
    Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
    Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
    Task<bool> DeleteVehicleAsync(int id);
    Task<Questionnaire> SaveQuestionnaireAsync(QuestionnaireDTO dto);
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

    public async Task<List<Vehicle>> SearchVehiclesAsync(VehicleFilterDto filters, int pageView, int offset, string? sortOrder = null)
    {
        try
        {
            return await _repository.SearchVehiclesAsync(filters, pageView, offset, sortOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vehicles");
            throw;
        }
    }

    public async Task<Questionnaire> SaveQuestionnaireAsync(QuestionnaireDTO dto)
    {
        try
        {
            return await _repository.SaveQuestionnaireAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving questionnaire");
            throw;
        }
    }

    public async Task<int> GetTotalCountAsync(VehicleFilterDto filterDto)
    {
        try
        {
            return await _repository.GetTotalCountAsync(filterDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicles count");
            throw;
        }
    }
    public async Task<Dictionary<string, int>> GetGearboxCountsAsync(VehicleFilterDto filterDto)
    { 
        try
        {
            return await _repository.GetGearboxCountsAsync(filterDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicles gearboxes");
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetAvailableColorsCountAsync(VehicleFilterDto filterDto)
    {
        try
        {
            return await _repository.GetColorsCountsAsync(filterDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicles colors");
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