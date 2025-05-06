using AutoFiCore.Models;
using AutoFiCore.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AutoFiCore.Controllers;

[ApiController]
[Route("[controller]")]

public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehicleController> _logger;

    public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAllVehicles([FromQuery] int pageView, [FromQuery] int offset)
    {
        try
        {
            if (pageView <= 0)
                return BadRequest("'pageView' must be greater than 0.");

            if (offset < 0)
                return BadRequest("'offset' must be 0 or greater.");

            var vehicles = await _vehicleService.GetAllVehiclesAsync(pageView, offset);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            return StatusCode(500, "An error occurred while retrieving vehicles");
        }
    }
    [HttpGet("by-make")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiclesByMake([FromQuery] int pageView, [FromQuery] int offset, [FromQuery] string make)
    {
        try
        {
            if (pageView <= 0)
                return BadRequest("'pageView' must be greater than 0.");

            if (offset < 0)
                return BadRequest("'offset' must be 0 or greater.");

            if (string.IsNullOrWhiteSpace(make)) return BadRequest("'make' is required.");

            var vehicles = await _vehicleService.GetVehiclesByMakeAsync(pageView, offset, make);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            return StatusCode(500, "An error occurred while retrieving vehicles");
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicleById(int id)
    {
        try
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID {id} not found");
            }
            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the vehicle");
        }
    }

    [HttpGet("vin/{vin}")]
    public async Task<ActionResult<Vehicle>> GetVehicleByVin(string vin)
    {
        try
        {
            var vehicle = await _vehicleService.GetVehicleByVinAsync(vin);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with VIN {vin} not found");
            }
            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle with VIN {Vin}", vin);
            return StatusCode(500, "An error occurred while retrieving the vehicle");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Vehicle>> CreateVehicle(Vehicle vehicle)
    {
        try
        {
            var existingVehicle = await _vehicleService.GetVehicleByVinAsync(vehicle.Vin);
            if (existingVehicle != null)
            {
                return BadRequest($"Vehicle with VIN {vehicle.Vin} already exists");
            }

            var createdVehicle = await _vehicleService.CreateVehicleAsync(vehicle);
            return CreatedAtAction(nameof(GetVehicleById), new { id = createdVehicle.Id }, createdVehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle with VIN {Vin}", vehicle.Vin);
            return StatusCode(500, "An error occurred while creating the vehicle");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Vehicle>> UpdateVehicle(int id, Vehicle vehicle)
    {
        try
        {
            if (id != vehicle.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingVehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (existingVehicle == null)
            {
                return NotFound($"Vehicle with ID {id} not found");
            }

            var updatedVehicle = await _vehicleService.UpdateVehicleAsync(vehicle);
            return Ok(updatedVehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the vehicle");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(int id)
    {
        try
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result)
            {
                return NotFound($"Vehicle with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the vehicle");
        }
    }
}
