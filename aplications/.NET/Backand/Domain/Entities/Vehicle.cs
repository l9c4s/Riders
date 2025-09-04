using Domain.Enum;

namespace Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public VehicleType Type { get; set; }
        public Guid GarageId { get; set; }
        public Garage? Garage { get; set; }
    }
}
