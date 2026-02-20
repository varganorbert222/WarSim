namespace WarSim.Domain.Units
{
    public class Infantry : LandUnit
    {
        /// <summary>
        /// Number of soldiers in the unit (optional).
        /// </summary>
        public int? Strength
        {
            get; set;
        }

        public string UnitRole { get; set; } = "Infantry";
    }
}
