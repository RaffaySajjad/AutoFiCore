using AutoFiCore.Models;

namespace AutoFiCore.Data;

public interface IVehicleRepository
{
    Task<VehicleListResult> GetAllVehiclesAsync(int pageView, int offset);
    Task<VehicleListResult> GetVehiclesByMakeAsync(int pageView, int offset, string make);
    Task<VehicleListResult> GetVehiclesByModelAsync(int pageView, int offset, string model);
    Task<VehicleListResult> SearchVehiclesAsync(int pageView, int offset, string? make = null, string? model = null, decimal? startPrice = null, decimal? endPrice = null);
    Task<List<string>> GetAllVehicleMakes();
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
    Task<bool> UpdateVehicleAsync(Vehicle vehicle);
    Task<bool> DeleteVehicleAsync(int id);
} 