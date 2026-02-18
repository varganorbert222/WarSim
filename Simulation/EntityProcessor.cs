using System;
using WarSim.Domain;
using WarSim.Domain.Projectiles;
using WarSim.Domain.Units;

namespace WarSim.Simulation
{
    public class EntityProcessor : IEntityProcessor
    {
        public Unit CloneUnit(Unit u)
        {
            if (u is Aircraft a)
            {
                var n = new Aircraft(a.Type)
                {
                    Name = a.Name,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Heading = a.Heading,
                    Status = a.Status,
                    MaxAltitude = a.MaxAltitude,
                    Airspeed = a.Airspeed,
                    Capacity = a.Capacity
                };
                n.Id = a.Id;
                return n;
            }

            if (u is Helicopter h)
            {
                var n = new Helicopter()
                {
                    Name = h.Name,
                    Latitude = h.Latitude,
                    Longitude = h.Longitude,
                    Heading = h.Heading,
                    Status = h.Status,
                    Airspeed = h.Airspeed
                };
                n.Id = h.Id;
                return n;
            }

            if (u is Vehicle v)
            {
                var n = new Vehicle(v.Type)
                {
                    Name = v.Name,
                    Latitude = v.Latitude,
                    Longitude = v.Longitude,
                    Heading = v.Heading,
                    Status = v.Status,
                    GroundSpeed = v.GroundSpeed,
                    Crew = v.Crew
                };
                n.Id = v.Id;
                return n;
            }

            if (u is Infantry i)
            {
                var n = new Infantry()
                {
                    Name = i.Name,
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Heading = i.Heading,
                    Status = i.Status,
                    Strength = i.Strength
                };
                n.Id = i.Id;
                return n;
            }

            if (u is Ship s)
            {
                var n = new Ship(s.Type)
                {
                    Name = s.Name,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Heading = s.Heading,
                    Status = s.Status,
                    SpeedKnots = s.SpeedKnots,
                    Crew = s.Crew
                };
                n.Id = s.Id;
                return n;
            }

            throw new NotSupportedException($"Cloning unit type {u.GetType()} not supported");
        }

        public Projectile CloneProjectile(Projectile p)
        {
            if (p is Missile m)
            {
                var n = new Missile()
                {
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Heading = m.Heading,
                    Speed = m.Speed,
                    Damage = m.Damage,
                    OwnerUnitId = m.OwnerUnitId,
                    GuidanceRangeMeters = m.GuidanceRangeMeters
                };
                n.Id = m.Id;
                return n;
            }

            if (p is Bullet b)
            {
                var n = new Bullet()
                {
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    Heading = b.Heading,
                    Speed = b.Speed,
                    Damage = b.Damage,
                    OwnerUnitId = b.OwnerUnitId,
                    CaliberMm = b.CaliberMm
                };
                n.Id = b.Id;
                return n;
            }

            if (p is Shell s)
            {
                var n = new Shell()
                {
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Heading = s.Heading,
                    Speed = s.Speed,
                    Damage = s.Damage,
                    OwnerUnitId = s.OwnerUnitId,
                    ShellMassKg = s.ShellMassKg
                };
                n.Id = s.Id;
                return n;
            }

            throw new NotSupportedException($"Cloning projectile type {p.GetType()} not supported");
        }

        public void UpdateUnitMovement(Unit unit, double deltaSeconds)
        {
            double speedMps = 0.0;
            switch (unit)
            {
                case LandUnit lu when lu.GroundSpeed.HasValue:
                    speedMps = lu.GroundSpeed.Value; // assumed m/s
                    break;
                case AirUnit au when au.Airspeed.HasValue:
                    speedMps = au.Airspeed.Value;
                    break;
                case SeaUnit su when su.SpeedKnots.HasValue:
                    speedMps = su.SpeedKnots.Value * 0.514444; // knots to m/s
                    break;
            }

            if (speedMps <= 0) return;

            var distance = speedMps * deltaSeconds; // meters
            MoveByDistance(unit, distance);
        }

        public void UpdateProjectileMovement(Projectile p, double deltaSeconds)
        {
            var distance = p.Speed * deltaSeconds;
            MoveByDistance(p, distance);
        }

        private static void MoveByDistance(dynamic obj, double distanceMeters)
        {
            double lat = obj.Latitude;
            double lon = obj.Longitude;
            double headingDeg = obj.Heading;

            var headingRad = headingDeg * Math.PI / 180.0;
            var dy = Math.Cos(headingRad) * distanceMeters; // north component
            var dx = Math.Sin(headingRad) * distanceMeters; // east component

            const double metersPerDegLat = 111320.0;
            var metersPerDegLon = metersPerDegLat * Math.Cos(lat * Math.PI / 180.0);

            var dLat = dy / metersPerDegLat;
            var dLon = metersPerDegLon == 0 ? 0 : dx / metersPerDegLon;

            obj.Latitude = lat + dLat;
            obj.Longitude = lon + dLon;
        }
}

}
