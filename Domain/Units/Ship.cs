namespace WarSim.Domain.Units
{
    public class Ship : SeaUnit
    {
        public ShipType Type
        {
            get; set;
        }

        public int? Crew
        {
            get; set;
        }

        public Ship(ShipType type)
        {
            Type = type;
        }
    }
}
