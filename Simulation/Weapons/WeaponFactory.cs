using System.Collections.Concurrent;
using WarSim.Domain;
using WarSim.Domain.Projectiles;

namespace WarSim.Simulation.Weapons
{
    /// <summary>
    /// Simple weapon factory with pooling for projectile instances to reduce GC churn.
    /// </summary>
    public class WeaponFactory
    {
        private readonly ConcurrentBag<Bullet> _bulletPool = new();
        private readonly ConcurrentBag<Shell> _shellPool = new();
        private readonly ConcurrentBag<Missile> _missilePool = new();

        public Projectile CreateProjectile(Commands.FireCommand cmd, double sourceLat, double sourceLon)
        {
            return cmd.ProjectileType switch
            {
                ProjectileType.Bullet => CreateBullet(cmd, sourceLat, sourceLon),
                ProjectileType.Shell => CreateShell(cmd, sourceLat, sourceLon),
                ProjectileType.Missile => CreateMissile(cmd, sourceLat, sourceLon),
                _ => throw new System.NotSupportedException($"Unsupported projectile type {cmd.ProjectileType}")
            };
        }

        private Bullet CreateBullet(Commands.FireCommand cmd, double sourceLat, double sourceLon)
        {
            if (!_bulletPool.TryTake(out var b))
            {
                b = new Bullet();
            }

            // If target coordinates provided, use them as initial position; otherwise use source coordinates
            b.Latitude = cmd.TargetLatitude ?? sourceLat;
            b.Longitude = cmd.TargetLongitude ?? sourceLon;
            b.Heading = cmd.Heading;
            b.Speed = cmd.Speed;
            b.Damage = cmd.Damage;
            b.OwnerUnitId = cmd.UnitId;
            b.CaliberMm = 7.62;
            return b;
        }

        private Shell CreateShell(Commands.FireCommand cmd, double sourceLat, double sourceLon)
        {
            if (!_shellPool.TryTake(out var s))
            {
                s = new Shell();
            }

            s.Latitude = cmd.TargetLatitude ?? sourceLat;
            s.Longitude = cmd.TargetLongitude ?? sourceLon;
            s.Heading = cmd.Heading;
            s.Speed = cmd.Speed;
            s.Damage = cmd.Damage;
            s.OwnerUnitId = cmd.UnitId;
            s.ShellMassKg = 10.0;
            return s;
        }

        private Missile CreateMissile(Commands.FireCommand cmd, double sourceLat, double sourceLon)
        {
            if (!_missilePool.TryTake(out var m))
            {
                m = new Missile();
            }

            m.Latitude = cmd.TargetLatitude ?? sourceLat;
            m.Longitude = cmd.TargetLongitude ?? sourceLon;
            m.Heading = cmd.Heading;
            m.Speed = cmd.Speed;
            m.Damage = cmd.Damage;
            m.OwnerUnitId = cmd.UnitId;
            m.GuidanceRangeMeters = 5000.0;
            return m;
        }

        public void ReturnProjectile(Projectile p)
        {
            switch (p)
            {
                case Bullet b:
                    _bulletPool.Add(b);
                    break;
                case Shell s:
                    _shellPool.Add(s);
                    break;
                case Missile m:
                    _missilePool.Add(m);
                    break;
            }
        }
    }
}
