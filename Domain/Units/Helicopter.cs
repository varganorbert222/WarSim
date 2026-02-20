namespace WarSim.Domain.Units
{
    public class Helicopter : AirUnit
    {
        public Helicopter(HelicopterSubcategory subcategory)
        {
            UnitCategory = UnitCategory.HELICOPTER;
            Subcategory = subcategory.ToString();
        }

        public Helicopter() : this(HelicopterSubcategory.UtilityHelicopter) { }
    }
}
