using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WarSim.Domain;
using WarSim.Domain.Projectiles;
using WarSim.Domain.Units;

namespace WarSim.Simulation
{
    /// <summary>
    /// Simple simulation engine that performs movement updates for units and projectiles.
    /// It follows a snapshot-update-swap approach: reads a snapshot, computes a new snapshot and updates the world state.
    /// Designed to be extended with combat, AI, frontlines, and more.
    /// </summary>
    public class SimulationEngine : ISimulationEngine
    {
        private readonly Services.WorldStateService _world;
        private readonly ILogger<SimulationEngine> _logger;
        private readonly IEntityProcessor _processor;
        private readonly IAIProcessor _ai;
        private readonly IWeaponSystem _weapons;
        private readonly WarSim.Simulation.Weapons.WeaponFactory _weaponFactory;

        public SimulationEngine(Services.WorldStateService world, ILogger<SimulationEngine> logger, IEntityProcessor processor, IAIProcessor ai, IWeaponSystem weapons, WarSim.Simulation.Weapons.WeaponFactory weaponFactory)
        {
            _world = world;
            _logger = logger;
            _processor = processor;
            _ai = ai;
            _weapons = weapons;
            _weaponFactory = weaponFactory;
        }

        public Task TickAsync(double deltaSeconds)
        {
            // Get immutable snapshot
            var snapshot = _world.GetSnapshot();

            var units = snapshot.Units.Select(u => _processor.CloneUnit(u)).ToList();
            var projectiles = snapshot.Projectiles.Select(p => _processor.CloneProjectile(p)).ToList();


            var newProjectiles = new System.Collections.Concurrent.ConcurrentBag<Projectile>();
            var newCommands = new System.Collections.Concurrent.ConcurrentBag<Commands.ICommand>();

            // Parallel update units
            Parallel.ForEach(units, unit =>
            {
                try
                {

                    // First let AI make decisions (may return commands)
                    var cmds = _ai.ProcessUnit(unit, snapshot);
                    if (cmds != null)
                    {
                        foreach (var c in cmds) newCommands.Add(c);
                    }

                    // Movement update based on possibly modified unit state
                    _processor.UpdateUnitMovement(unit, deltaSeconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating unit {UnitId}", unit.Id);
                }
            });

            // Merge newly fired projectiles into the projectile list
            projectiles.AddRange(newProjectiles);

            // Collect and process commands produced by AI
            // Parallel update projectiles
            Parallel.ForEach(projectiles, proj =>
            {
                try
                {
                    _processor.UpdateProjectileMovement(proj, deltaSeconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating projectile {ProjectileId}", proj.Id);
                }
            });

            // Process commands (move/fire) after unit updates
            while (!newCommands.IsEmpty)
            {
                if (!newCommands.TryTake(out var cmd)) break;
                switch (cmd)
                {
                    case Commands.MoveCommand mc:
                    {
                        var target = units.FirstOrDefault(u => u.Id == mc.UnitId);
                        if (target != null)
                        {
                            if (mc.Heading.HasValue) target.Heading = mc.Heading.Value;
                            if (mc.Latitude.HasValue) target.Latitude = mc.Latitude.Value;
                            if (mc.Longitude.HasValue) target.Longitude = mc.Longitude.Value;
                            if (mc.Speed.HasValue)
                            {
                                switch (target)
                                {
                                    case LandUnit lu: lu.GroundSpeed = mc.Speed; break;
                                    case AirUnit au: au.Airspeed = mc.Speed; break;
                                    case SeaUnit su: su.SpeedKnots = mc.Speed; break;
                                }
                            }
                            target.Status = UnitStatus.Moving;
                        }
                        break;
                    }
                    case Commands.FireCommand fc:
                    {
                        var shooter = units.FirstOrDefault(u => u.Id == fc.UnitId);
                        var proj = _weaponFactory.CreateProjectile(fc, shooter?.Latitude ?? 0.0, shooter?.Longitude ?? 0.0);
                        projectiles.Add(proj);
                        break;
                    }
                }
            }

            // After processing commands, check projectile collisions and apply effects
            projectiles = _weapons.ProcessProjectiles(projectiles, units, snapshot.Factions);

            var newState = new WorldState(units, projectiles, snapshot.Factions, snapshot.Tick + 1);
            _world.UpdateState(newState);

            return Task.CompletedTask;
        }

        private static Unit CloneUnit(Unit u)
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

        private static Projectile CloneProjectile(Projectile p)
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

        private static void UpdateUnitMovement(Unit unit, double deltaSeconds)
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

            // Simple flat-earth movement approximation
            var distance = speedMps * deltaSeconds; // meters
            MoveByDistance(unit, distance);
        }

        private static void UpdateProjectileMovement(Projectile p, double deltaSeconds)
        {
            var distance = p.Speed * deltaSeconds;
            MoveByDistance(p, distance);
        }

        private static void MoveByDistance(dynamic obj, double distanceMeters)
        {
            // dynamic allows using Unit or Projectile which share Lat/Lon/Heading
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
