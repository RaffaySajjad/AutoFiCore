using Microsoft.AspNetCore.Mvc;
using AutoFiCore.Data;
using AutoFiCore.Models;
using AutoFiCore.Services;

namespace AutoFiCore.Controllers;

[ApiController]
[Route("[controller]")]
public class LoanCalculationController : ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly ILogger<LoanCalculationController> _logger;

    public LoanCalculationController(ILoanService loanService, ILogger<LoanCalculationController> logger)
    {
        _loanService = loanService;
        _logger = logger;
    }

    [HttpPost("CalculateLoan")]
    public async Task<ActionResult<LoanCalculation>> Calculate([FromBody] LoanRequest request)
    {
        try
        {
            var result = await _loanService.CalculateLoanAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in loan calculation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calculating loan");
            return StatusCode(500, "Internal server error");
        }
    }
}