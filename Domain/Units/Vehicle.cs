using WarSim.Domain;

namespace WarSim.Domain.Units
{
    public class Vehicle : LandUnit
    {
        public VehicleType Type { get; set; }

        public int? Crew { get; set; }

        public Vehicle(VehicleType type)
        {
            Type = type;
        }
    }
}
