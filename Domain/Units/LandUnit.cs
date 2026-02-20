namespace WarSim.Domain.Units
{
    public abstract class LandUnit : Unit
    {
        public override string Category => "Land";

        /// <summary>
        /// Ground speed in m/s (optional).
        /// </summary>
        public double? GroundSpeed
        {
            get; set;
        }
    }
}
