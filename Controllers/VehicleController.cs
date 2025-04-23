using Microsoft.AspNetCore.Mvc;
using AutoFiCore.Data;
using AutoFiCore.Models;

namespace AutoFiCore.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<VehicleController> _logger;

    public VehicleController(IVehicleRepository vehicleRepository, ILogger<VehicleController> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    [HttpGet(Name = "GetVehicles")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> Get()
    {
        return Ok(await _vehicleRepository.GetAllVehiclesAsync());
    }

    [HttpGet("{id}", Name = "GetVehicleById")]
    public async Task<ActionResult<Vehicle>> Get(int id)
    {
        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }
        return Ok(vehicle);
    }

    [HttpPost]
    public async Task<ActionResult<Vehicle>> Post(Vehicle vehicle)
    {
        var result = await _vehicleRepository.AddVehicleAsync(vehicle);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return BadRequest();
        }

        var success = await _vehicleRepository.UpdateVehicleAsync(vehicle);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _vehicleRepository.DeleteVehicleAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
