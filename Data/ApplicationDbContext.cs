using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoFiCore.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<Drivetrain> Drivetrains { get; set; } = null!;
    public DbSet<Engine> Engines { get; set; } = null!;
    public DbSet<FuelEconomy> FuelEconomies { get; set; } = null!;
    public DbSet<VehiclePerformance> VehiclePerformances { get; set; } = null!;
    public DbSet<Measurements> Measurements { get; set; }

    public DbSet<VehicleOptions> VehicleOptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Configure constraints
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Vin).IsRequired().HasMaxLength(17);
            entity.Property(e => e.Make).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Mileage).IsRequired();
            entity.Property(e => e.Color).HasMaxLength(30);
            entity.Property(e => e.FuelType).HasMaxLength(20);
            entity.Property(e => e.Transmission).HasMaxLength(20);
        });

        modelBuilder.Entity<Drivetrain>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Transmission).IsRequired().HasMaxLength(50);
            ;
        });

        modelBuilder.Entity<Engine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(25);
            entity.Property(e => e.Size).HasMaxLength(4);
            entity.Property(e => e.Horsepower).IsRequired();
            entity.Property(e => e.TorqueFtLBS);
            entity.Property(e => e.TorqueRPM);
            entity.Property(e => e.Valves);
            entity.Property(e => e.CamType).HasMaxLength(25);
        });

        modelBuilder.Entity<FuelEconomy>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.FuelTankSize);
            entity.Property(f => f.CombinedMPG).IsRequired();
            entity.Property(f => f.CityMPG).IsRequired();
            entity.Property(f => f.HighwayMPG).IsRequired();
            entity.Property(f => f.CO2Emissions).IsRequired();
        });

        modelBuilder.Entity<VehiclePerformance>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.ZeroTo60MPH).IsRequired();
        });

        modelBuilder.Entity<Measurements>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Doors).IsRequired();
            entity.Property(v => v.MaximumSeating).IsRequired();
            entity.Property(v => v.HeightInches).IsRequired();
            entity.Property(v => v.WidthInches).IsRequired();
            entity.Property(v => v.LengthInches).IsRequired();
            entity.Property(v => v.WheelbaseInches).IsRequired();
            entity.Property(v => v.GroundClearance).IsRequired();
            entity.Property(v => v.CargoCapacityCuFt);
            entity.Property(v => v.CurbWeightLBS).IsRequired();
        });

        modelBuilder.Entity<VehicleOptions>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Options).IsRequired();
        });

        // Configure relationships and set up cascade delete
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Drivetrain)
            .WithOne(d => d.Vehicle)
            .HasForeignKey<Drivetrain>(d => d.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Engine)
            .WithOne(e => e.Vehicle)
            .HasForeignKey<Engine>(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.FuelEconomy)
            .WithOne(f => f.Vehicle)
            .HasForeignKey<FuelEconomy>(f => f.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.VehiclePerformance)
            .WithOne(p => p.Vehicle)
            .HasForeignKey<VehiclePerformance>(p => p.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Measurements)
            .WithOne(m => m.Vehicle)
            .HasForeignKey<Measurements>(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasMany(v => v.VehicleOptions)
            .WithMany(o => o.Vehicle)
            .UsingEntity(j => j.ToTable("Vehicle_VehicleOptions_Mapping"));
    }
}