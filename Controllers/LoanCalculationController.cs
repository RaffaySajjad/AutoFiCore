using Microsoft.AspNetCore.Mvc;
using AutoFiCore.Data;
using AutoFiCore.Models;

namespace AutoFiCore.Controllers;

[ApiController]
[Route("[controller]")]
public class LoanCalculationController : ControllerBase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<LoanCalculationController> _logger;

    public LoanCalculationController(IVehicleRepository vehicleRepository, ILogger<LoanCalculationController> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    [HttpPost(Name = "CalculateLoan")]
    public async Task<ActionResult<LoanCalculation>> Calculate(LoanRequest request)
    {
        if (request.LoanAmount <= 0 || request.InterestRate <= 0 || request.LoanTermMonths <= 0)
        {
            return BadRequest("Loan amount, interest rate, and loan term must be greater than zero");
        }

        // Verify vehicle exists
        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.VehicleId);
        if (vehicle == null)
        {
            return NotFound($"Vehicle with ID {request.VehicleId} not found");
        }

        // Monthly interest rate
        decimal monthlyRate = request.InterestRate / 100 / 12;
        
        // Calculate monthly payment using the loan formula
        decimal monthlyPayment = request.LoanAmount * 
            (monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), request.LoanTermMonths)) /
            ((decimal)Math.Pow((double)(1 + monthlyRate), request.LoanTermMonths) - 1);
        
        decimal totalCost = monthlyPayment * request.LoanTermMonths;
        decimal totalInterest = totalCost - request.LoanAmount;

        var calculation = new LoanCalculation
        {
            Id = new Random().Next(1, 1000), // Just for demo purposes
            VehicleId = request.VehicleId,
            LoanAmount = request.LoanAmount,
            InterestRate = request.InterestRate,
            LoanTermMonths = request.LoanTermMonths,
            MonthlyPayment = Math.Round(monthlyPayment, 2),
            TotalInterest = Math.Round(totalInterest, 2),
            TotalCost = Math.Round(totalCost, 2)
        };

        return calculation;
    }

    public class LoanRequest
    {
        public int VehicleId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanTermMonths { get; set; }
    }
} 