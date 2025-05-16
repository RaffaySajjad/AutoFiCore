namespace AutoFiCore.Models
{
    public class VehicleListResult
    {
        public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public int TotalCount { get; set; }
    }

}
