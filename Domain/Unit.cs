namespace WarSim.Domain
{
    /// <summary>
    /// Base class for all units in the simulation.
    /// Designed to be lightweight and easily extended by specific unit types.
    /// </summary>
    public abstract class Unit
    {
        protected Unit()
        {
            Id = Guid.NewGuid();
        }

        // Keep setter to allow creating snapshot clones with the same identity
        public Guid Id
        {
            get; set;
        }

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Geographic position - latitude in degrees.
        /// </summary>
        public double Latitude
        {
            get; set;
        }

        /// <summary>
        /// Geographic position - longitude in degrees.
        /// </summary>
        public double Longitude
        {
            get; set;
        }

        /// <summary>
        /// Heading in degrees (0-360).
        /// </summary>
        public double Heading
        {
            get; set;
        }

        public UnitStatus Status { get; set; } = UnitStatus.Idle;

        /// <summary>
        /// Current health of the unit. When <= 0 the unit is considered destroyed.
        /// </summary>
        public double Health { get; set; } = 100.0;

        /// <summary>
        /// Faction identifier. Units with the same faction are considered allies.
        /// </summary>
        public int FactionId { get; set; } = 0;

        /// <summary>
        /// Vision range in meters; used by AI for line-of-sight / target selection.
        /// </summary>
        public double VisionRangeMeters { get; set; } = 2000.0;

        /// <summary>
        /// Main category of the unit (AIRPLANE, HELICOPTER, GROUND_UNIT, SHIP, STRUCTURE).
        /// </summary>
        public UnitCategory UnitCategory
        {
            get; set;
        }

        /// <summary>
        /// Subcategory stored as string to accommodate all enum types.
        /// Use GetSubcategory<T>() helper to parse to specific enum.
        /// </summary>
        public string Subcategory { get; set; } = string.Empty;

        /// <summary>
        /// Weapon loadout for this unit (managed by weapon system)
        /// </summary>
        public List<Weapons.WeaponSlot> WeaponSlots { get; set; } = new();

        public T? GetSubcategory<T>() where T : struct, Enum
        {
            if (string.IsNullOrEmpty(Subcategory))
            {
                return null;
            }

            return Enum.TryParse<T>(Subcategory, true, out var result) ? result : null;
        }

        /// <summary>
        /// Category string for easier filtering in UIs (e.g. "Air", "Land", "Sea").
        /// Subclasses can override to provide a more specific value.
        /// </summary>
        public virtual string Category => UnitCategory.ToString();

        /// <summary>
        /// Get total ammo percentage across all weapons
        /// </summary>
        public double GetAmmoPercent()
        {
            if (!WeaponSlots.Any())
            {
                return 1.0;
            }

            var totalMax = WeaponSlots.Sum(w => w.CurrentAmmo + (w.CurrentMagazine * 10)); // rough estimate
            var totalCurrent = WeaponSlots.Sum(w => w.CurrentAmmo);
            return totalMax > 0 ? (double)totalCurrent / totalMax : 0;
        }
    }
}
