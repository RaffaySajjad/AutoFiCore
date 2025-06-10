using AutoFiCore.Models;
using AutoFiCore.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AutoFiCore.Utilities;
using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.Json;
namespace AutoFiCore.Controllers;

using Newtonsoft.Json;
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
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAllVehiclesByStatus([FromQuery] int pageView, [FromQuery] int offset, [FromQuery] string? status = null)
    {
        try
        {
            var validationError = Validator.ValidatePagination(pageView, offset);
            if (validationError != null)
                return BadRequest(validationError);


            var vehicles = await _vehicleService.GetAllVehiclesByStatusAsync(pageView, offset, status);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            return StatusCode(500, "An error occurred while retrieving vehicles");
        }
    }
    [HttpGet("features")]
    public async Task<ActionResult<VehicleModelJSON>> GetCarFeatures([FromQuery] string make, [FromQuery] string model)
    {
        make = make.Trim();
        var makeValidator = Validator.ValidateMakeOrModel(make);
        if (makeValidator != null)
            return BadRequest(makeValidator);

        model = model.Trim();
        var modelValidator = Validator.ValidateMakeOrModel(model);
        if (modelValidator != null)
            return BadRequest(modelValidator);

        var carFeatures = await _vehicleService.GetAllCarFeaturesAsync();

        if (carFeatures == null || carFeatures.Count == 0)
            return NotFound("No car features found.");

        var match = _vehicleService.GetCarFeature(carFeatures, make, model);

        if (match == null)
            return NotFound($"No data found for {make} {model}.");

        var normalized = NormalizeInput.NormalizeCarFeatures(match);

        return Ok(normalized);
    }
    [HttpGet("get-colors")]
    public async Task<ActionResult<List<string>>> GetAllCarColors()
    {
        try
        {
            var result = await _vehicleService.GetDistinctColorsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all car colors");
            throw;
        }
    }

    [HttpGet("by-make")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiclesByMake([FromQuery] int pageView, [FromQuery] int offset, [FromQuery] string make)
    {
        try
        {
            make = make.Trim();
            var paginationValidator = Validator.ValidatePagination(pageView, offset);

            if (paginationValidator != null)
                return BadRequest(paginationValidator);

            var makeValidator = Validator.ValidateMakeOrModel(make);
            if (makeValidator != null)
                return BadRequest(makeValidator);

            var vehicles = await _vehicleService.GetVehiclesByMakeAsync(pageView, offset, make);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            return StatusCode(500, "An error occurred while retrieving vehicles");
        }
    }
    [HttpGet("search-vehicles")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> SearchVehicles(
        [FromQuery] int pageView,
        [FromQuery] int offset,
        [FromQuery] string? make,
        [FromQuery] string? model,
        [FromQuery] decimal? startPrice,
        [FromQuery] decimal? endPrice,
        [FromQuery] int? mileage,
        [FromQuery] int? startYear,
        [FromQuery] int? endYear,
        [FromQuery] string? sortOrder = null,
        [FromQuery] string? gearbox = null,
        [FromQuery] string? selectedColors = null,
        [FromQuery] string? status = null
        )
    {
        try
        {
            make = NormalizeInput.NormalizeMakeModel(make);
            model = NormalizeInput.NormalizeMakeModel(model);
            gearbox = NormalizeInput.NormalizeGearboxColors(gearbox);
            selectedColors = NormalizeInput.NormalizeGearboxColors(selectedColors);
            status = NormalizeInput.NormalizeStatus(status);

            var mileageValidator = Validator.ValidateMileage(mileage);
            if (mileageValidator != null)
                return BadRequest(mileageValidator);

            var priceValidator = Validator.ValidatePrice(startPrice, endPrice);
            if (!priceValidator)
                return BadRequest(priceValidator);

            var vehicles = await _vehicleService.SearchVehiclesAsync(pageView, offset, make, model, startPrice, endPrice, mileage, startYear, endYear, sortOrder, gearbox, selectedColors, status);
            return Ok(vehicles);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vehicles");
            return StatusCode(500, "An error occured while searching vehicles");
        }
    }

    [HttpGet("by-model")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiclesByModel([FromQuery] int pageView, [FromQuery] int offset, [FromQuery] string model)
    {
        try
        {
            model = model.Trim();
            var paginationValidator = Validator.ValidatePagination(pageView, offset);

            if (paginationValidator != null)
                return BadRequest(paginationValidator);

            var modelValidator = Validator.ValidateMakeOrModel(model);
            if (modelValidator != null)
                return BadRequest(modelValidator);

            var vehicles = await _vehicleService.GetVehiclesByModelAsync(pageView, offset, model);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            return StatusCode(500, "An error occurred while retrieving vehicles");
        }
    }

    [HttpGet("get-makes")]
    public async Task<ActionResult<List<string>>> GetAllMakes()
    {
        try
        {
            var makes = await _vehicleService.GetAllVehiclesMakesAsync();
            return Ok(makes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle makes");
            return StatusCode(500, "An error occurred while retrieving vehicle makes");
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
