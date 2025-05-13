using AutoFiCore.Models;

namespace AutoFiCore.Data;

public interface IVehicleRepository
{
    Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(int pageView, int offset);
    Task<IEnumerable<Vehicle>> GetVehiclesByMakeAsync(int pageView, int offset, string make);
    Task<IEnumerable<Vehicle>> GetVehiclesByModelAsync(int pageView, int offset, string model);
    Task <List<string>> GetAllVehicleMakes();
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
    Task<bool> UpdateVehicleAsync(Vehicle vehicle);
    Task<bool> DeleteVehicleAsync(int id);
} 