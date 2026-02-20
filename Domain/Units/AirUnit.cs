namespace WarSim.Domain.Units
{
    public abstract class AirUnit : Unit
    {
        public override string Category => "Air";

        /// <summary>
        /// Typical maximum altitude in meters (optional).
        /// </summary>
        public double? MaxAltitude
        {
            get; set;
        }

        /// <summary>
        /// Typical airspeed in m/s (optional).
        /// </summary>
        public double? Airspeed
        {
            get; set;
        }
    }
}
