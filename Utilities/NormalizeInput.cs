using AutoFiCore.Models;

namespace AutoFiCore.Utilities
{
    public class NormalizeInput
    {
        public static string? NormalizeMakeModel(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            if (input.Equals("Any_Makes", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("Any_Models", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return input;
        }

        public static string? NormalizeGearbox(string? gearbox)
        {
            if (string.IsNullOrWhiteSpace(gearbox) ||
                string.Equals(gearbox.Trim(), "Any", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return gearbox.Trim();
        }

        public static object NormalizeCarFeatures(VehicleModelJSON vehicle)
        {
            if (vehicle?.Features == null)
                return new { };

            return new
            {
                make = vehicle.Make,
                model = vehicle.Model,
                year = vehicle.Year,
                features = new
                {
                    drivetrain = vehicle.Features.Drivetrain == null ? null : new
                    {
                        type = vehicle.Features.Drivetrain.Type,
                        transmission = vehicle.Features.Drivetrain.Transmission
                    },
                    engine = vehicle.Features.Engine == null ? null : new
                    {
                        type = vehicle.Features.Engine.Type,
                        size = vehicle.Features.Engine.Size,
                        horsepower = vehicle.Features.Engine.Horsepower,
                        torqueFtLBS = vehicle.Features.Engine.TorqueFtLBS,
                        torqueRPM = vehicle.Features.Engine.TorqueRPM,
                        valves = vehicle.Features.Engine.Valves,
                        camType = vehicle.Features.Engine.CamType
                    },
                    fuelEconomy = vehicle.Features.FuelEconomy == null ? null : new
                    {
                        fuelTankSize = vehicle.Features.FuelEconomy.FuelTankSize,
                        combinedMPG = vehicle.Features.FuelEconomy.CombinedMPG,
                        cityMPG = vehicle.Features.FuelEconomy.CityMPG,
                        highwayMPG = vehicle.Features.FuelEconomy.HighwayMPG,
                        cO2Emissions = vehicle.Features.FuelEconomy.CO2Emissions
                    },
                    performance = vehicle.Features.Performance == null ? null : new
                    {
                        zeroTo60MPH = vehicle.Features.Performance.ZeroTo60MPH
                    },
                    measurements = vehicle.Features.Measurements == null ? null : new
                    {
                        doors = vehicle.Features.Measurements.Doors,
                        maximumSeating = vehicle.Features.Measurements.MaximumSeating,
                        heightInches = vehicle.Features.Measurements.HeightInches,
                        widthInches = vehicle.Features.Measurements.WidthInches,
                        lengthInches = vehicle.Features.Measurements.LengthInches,
                        wheelbaseInches = vehicle.Features.Measurements.WheelbaseInches,
                        groundClearance = vehicle.Features.Measurements.GroundClearance,
                        cargoCapacityCuFt = vehicle.Features.Measurements.CargoCapacityCuFt,
                        curbWeightLBS = vehicle.Features.Measurements.CurbWeightLBS
                    },
                    options = vehicle.Features.Options
                }
            };
        }


    }
    }
