using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFiCore.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
} 