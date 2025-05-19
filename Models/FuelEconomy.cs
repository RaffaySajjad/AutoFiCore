using System.ComponentModel.DataAnnotations;

namespace AutoFiCore.Models
{
    public class FuelEconomy
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }

        [Range(0, int.MaxValue)]
        public int? FuelTankSize { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CombinedMPG { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CityMPG { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int HighwayMPG { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int CO2Emissions { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int BetterCapacityKWH { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int RangeMiles { get; set; }

        public Vehicle? Vehicle { get; set; } = null;
    }
}
