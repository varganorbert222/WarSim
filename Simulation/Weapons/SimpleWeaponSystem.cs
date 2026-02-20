using WarSim.Domain;
using WarSim.Domain.Projectiles;
using WarSim.Logging;

namespace WarSim.Simulation.Weapons
{
    public class SimpleWeaponSystem : IWeaponSystem
    {
        private readonly Random _rand = new();
        private readonly WeaponFactory _factory;

        public SimpleWeaponSystem(WeaponFactory factory)
        {
            _factory = factory;
        }

        public IEnumerable<Projectile> TryFire(Unit unit, WorldState snapshot)
        {
            // Simple placeholder: aircraft have small chance to fire bullets; vehicles infrequently fire shells.
            var list = new List<Projectile>();

            if (unit is WarSim.Domain.Units.Aircraft)
            {
                if (_rand.NextDouble() < 0.01) // 1% per tick
                {
                    var b = new Bullet()
                    {
                        Latitude = unit.Latitude,
                        Longitude = unit.Longitude,
                        Heading = unit.Heading,
                        Speed = 400.0,
                        Damage = 10.0,
                        OwnerUnitId = unit.Id,
                        CaliberMm = 7.62
                    };
                    list.Add(b);
                }
            }
            else if (unit is WarSim.Domain.Units.Vehicle)
            {
                if (_rand.NextDouble() < 0.002) // 0.2% per tick
                {
                    var s = new Shell()
                    {
                        Latitude = unit.Latitude,
                        Longitude = unit.Longitude,
                        Heading = unit.Heading,
                        Speed = 250.0,
                        Damage = 50.0,
                        OwnerUnitId = unit.Id,
                        ShellMassKg = 15.0
                    };
                    list.Add(s);
                }
            }

            return list;
        }

        public List<Projectile> ProcessProjectiles(List<Projectile> projectiles, List<Unit> units, IReadOnlyList<Faction> factions)
        {
            var remaining = new List<Projectile>();

            foreach (var p in projectiles)
            {
                // find nearest enemy target (not the owner and not same faction)
                Unit? target = null;
                double bestDist = double.MaxValue;
                // determine owner's faction if possible
                var owner = units.FirstOrDefault(x => x.Id == p.OwnerUnitId);
                var ownerFaction = owner?.FactionId;
                foreach (var u in units)
                {
                    if (u.Id == p.OwnerUnitId)
                    {
                        continue;
                    }

                    if (u.Status == UnitStatus.Destroyed)
                    {
                        continue;
                    }

                    if (ownerFaction.HasValue && u.FactionId == ownerFaction.Value)
                    {
                        continue; // skip friendlies
                    }

                    var d = DistanceMeters(p.Latitude, p.Longitude, u.Latitude, u.Longitude);
                    if (d < bestDist)
                    {
                        bestDist = d;
                        target = u;
                    }
                }

                bool hit = false;
                if (target != null)
                {
                    // threshold depends on projectile type
                    double threshold = p switch
                    {
                        Bullet => 10.0,
                        Shell => 30.0,
                        Missile => 50.0,
                        _ => 10.0
                    };

                    if (bestDist <= threshold)
                    {
                        // apply damage
                        target.Health -= p.Damage;

                        var projectileType = p switch
                        {
                            Bullet => "Bullet",
                            Shell => "Shell",
                            Missile => "Missile",
                            _ => "Projectile"
                        };

                        var ownerName = units.FirstOrDefault(u => u.Id == p.OwnerUnitId)?.Name ?? "Unknown";

                        if (target.Health <= 0)
                        {
                            target.Status = UnitStatus.Destroyed;
                            ConsoleColorLogger.Log("Combat", Microsoft.Extensions.Logging.LogLevel.Warning, $"⚔️ {ownerName} DESTROYED {target.Name} with {projectileType} (dealt {p.Damage} damage)");
                        }
                        else
                        {
                            ConsoleColorLogger.Log("Combat", Microsoft.Extensions.Logging.LogLevel.Information, $"💥 {ownerName} HIT {target.Name} with {projectileType} for {p.Damage} damage (HP: {target.Health:F1})");
                        }

                        // return projectile to pool
                        _factory.ReturnProjectile(p);
                        hit = true;
                    }
                }

                if (!hit)
                {
                    remaining.Add(p);
                }
            }

            return remaining;
        }

        private static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double metersPerDegLat = 111320.0;
            var dy = (lat2 - lat1) * metersPerDegLat;
            var avgLat = (lat1 + lat2) / 2.0 * Math.PI / 180.0;
            var metersPerDegLon = metersPerDegLat * Math.Cos(avgLat);
            var dx = (lon2 - lon1) * metersPerDegLon;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
    }
}
