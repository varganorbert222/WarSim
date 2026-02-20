namespace WarSim.Domain.Units
{
    public class Vehicle : LandUnit
    {
        public int? Crew
        {
            get; set;
        }

        public Vehicle(GroundUnitSubcategory subcategory)
        {
            UnitCategory = UnitCategory.GROUND_UNIT;
            Subcategory = subcategory.ToString();
        }

        public Vehicle() : this(GroundUnitSubcategory.ReconVehicle) { }
    }
}
