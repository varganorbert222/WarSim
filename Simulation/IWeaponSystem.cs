using System.Collections.Generic;
using WarSim.Domain;
using WarSim.Domain.Projectiles;

namespace WarSim.Simulation
{
    public interface IWeaponSystem
    {
        /// <summary>
        /// Evaluate whether the unit fires; return zero or more generated projectiles.
        /// </summary>
        IEnumerable<Projectile> TryFire(Unit unit, WorldState snapshot);

        /// <summary>
        /// Process projectiles for collisions and effects against units. Returns the list of remaining projectiles after processing.
        /// Implementations are responsible for returning collided projectiles to the WeaponFactory if pooling is used.
        /// </summary>
        List<Projectile> ProcessProjectiles(List<Projectile> projectiles, List<Unit> units, IReadOnlyList<Faction> factions);
    }
}
