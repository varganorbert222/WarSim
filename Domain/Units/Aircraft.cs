namespace WarSim.Domain.Units
{
    public class Aircraft : AirUnit
    {
        /// <summary>
        /// Number of crew or capacity (optional).
        /// </summary>
        public int? Capacity
        {
            get; set;
        }

        public Aircraft(AirplaneSubcategory subcategory)
        {
            UnitCategory = UnitCategory.AIRPLANE;
            Subcategory = subcategory.ToString();
        }

        public Aircraft() : this(AirplaneSubcategory.Fighter) { }
    }
}
