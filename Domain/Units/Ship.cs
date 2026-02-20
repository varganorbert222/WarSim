namespace WarSim.Domain.Units
{
    public class Ship : SeaUnit
    {
        public int? Crew
        {
            get; set;
        }

        public Ship(ShipSubcategory subcategory)
        {
            UnitCategory = UnitCategory.SHIP;
            Subcategory = subcategory.ToString();
        }

        public Ship() : this(ShipSubcategory.Frigate) { }
    }
}
