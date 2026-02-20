namespace WarSim.Domain.Units
{
    /// <summary>
    /// Represents a static structure (building, radar, SAM site, etc).
    /// Structures typically don't move but can be destroyed.
    /// </summary>
    public class Structure : Unit
    {
        public Structure(StructureSubcategory subcategory)
        {
            UnitCategory = UnitCategory.STRUCTURE;
            Subcategory = subcategory.ToString();
        }

        public Structure() : this(StructureSubcategory.MilitaryBuilding) { }

        /// <summary>
        /// Structures don't have speed.
        /// </summary>
        public override string Category => "STRUCTURE";
    }
}
