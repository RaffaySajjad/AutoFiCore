using Microsoft.EntityFrameworkCore;
using AutoFiCore.Models;

namespace AutoFiCore.Data;

public class AutoFiDbContext : DbContext
{
    public AutoFiDbContext(DbContextOptions<AutoFiDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<LoanCalculation> LoanCalculations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>()
            .HasKey(v => v.Id);

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<LoanCalculation>()
            .HasKey(lc => lc.Id);

        modelBuilder.Entity<LoanCalculation>()
            .Property(lc => lc.LoanAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<LoanCalculation>()
            .Property(lc => lc.InterestRate)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<LoanCalculation>()
            .Property(lc => lc.MonthlyPayment)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<LoanCalculation>()
            .Property(lc => lc.TotalInterest)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<LoanCalculation>()
            .Property(lc => lc.TotalCost)
            .HasColumnType("decimal(18,2)");
    }
} 