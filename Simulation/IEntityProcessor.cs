using WarSim.Domain;
using WarSim.Domain.Projectiles;

namespace WarSim.Simulation
{
    public interface IEntityProcessor
    {
        Unit CloneUnit(Unit u);
        Projectile CloneProjectile(Projectile p);
        void UpdateUnitMovement(Unit unit, double deltaSeconds);
        void UpdateProjectileMovement(Projectile p, double deltaSeconds);
    }
}
