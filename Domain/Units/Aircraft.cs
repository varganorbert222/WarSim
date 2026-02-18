using WarSim.Domain;

namespace WarSim.Domain.Units
{
    public class Aircraft : AirUnit
    {
        public AircraftType Type { get; set; }

        /// <summary>
        /// Number of crew or capacity (optional).
        /// </summary>
        public int? Capacity { get; set; }

        public Aircraft(AircraftType type)
        {
            Type = type;
        }
    }
}
