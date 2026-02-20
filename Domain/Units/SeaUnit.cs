namespace WarSim.Domain.Units
{
    public abstract class SeaUnit : Unit
    {
        public override string Category => "Sea";

        /// <summary>
        /// Nautical speed in knots (optional).
        /// </summary>
        public double? SpeedKnots
        {
            get; set;
        }
    }
}
