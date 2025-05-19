using System.ComponentModel.DataAnnotations;

namespace AutoFiCore.Models
{
    public class Measurements
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }

        [Required]
        [Range(2, int.MaxValue)]
        public int Doors { get; set; }

        [Required]
        [Range(2, int.MaxValue)]
        public int MaximumSeating { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int HeightInches { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int WidthInches { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int LengthInches { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int WheelbaseInches { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int GroundClearance { get; set; }

        [Range(0, int.MaxValue)]
        public int? CargoCapacityCuFt { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CargoWeightLBS { get; set; }
        public Vehicle? Vehicle { get; set; } = null;

    }
}
