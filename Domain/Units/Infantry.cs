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

        public Infantry()
        {
            UnitCategory = UnitCategory.GROUND_UNIT;
            Subcategory = GroundUnitSubcategory.Infantry.ToString();
        }
    }
}
