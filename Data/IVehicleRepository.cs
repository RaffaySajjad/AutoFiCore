using AutoFiCore.Models;

namespace AutoFiCore.Data;

public interface IVehicleRepository
{
    Task<VehicleListResult> GetAllVehiclesByStatusAsync(int pageView, int offset, string? status = null);
    Task<VehicleListResult> GetVehiclesByMakeAsync(int pageView, int offset, string make);
    Task<VehicleListResult> GetVehiclesByModelAsync(int pageView, int offset, string model);
    Task<List<string>> GetDistinctColorsAsync();
    Task<VehicleListResult> SearchVehiclesAsync(
        int pageView,
        int offset, 
        string? make = null, 
        string? model = null, 
        decimal? startPrice = null, 
        decimal? endPrice = null, 
        int? mileage = null,
        int? startYear = null,
        int? endYear = null,
        string? sortOrder = null,
        string? gearbox = null,
        string? selectedColors = null
        );
    VehicleModelJSON? GetCarFeature(List<VehicleModelJSON> carFeatures, string make, string model);
    Task<List<VehicleModelJSON>> GetAllCarFeaturesAsync();
    Task<List<string>> GetAllVehicleMakes();
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
    Task<bool> UpdateVehicleAsync(Vehicle vehicle);
    Task<bool> DeleteVehicleAsync(int id);
} 