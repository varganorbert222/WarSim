using System.Collections.Generic;
using WarSim.Domain.Projectiles;

namespace WarSim.Domain
{
    public sealed record WorldState(
        IReadOnlyList<Unit> Units,
        IReadOnlyList<Projectile> Projectiles,
        IReadOnlyList<Faction> Factions,
        long Tick
    );
}
