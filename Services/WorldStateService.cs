using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using WarSim.Domain;
using WarSim.Domain.Projectiles;
using WarSim.Domain.Units;

namespace WarSim.Services
{
    /// <summary>
    /// Holds and provides access to the authoritative world state. Uses snapshot-update-swap semantics.
    /// </summary>
    public class WorldStateService
    {
        private WorldState _state;
        private readonly ILogger<WorldStateService> _logger;

        public WorldStateService(ILogger<WorldStateService> logger)
        {
            _logger = logger;

            // Demo units using the new type hierarchy
            var a1 = new WarSim.Domain.Units.Aircraft(WarSim.Domain.AircraftType.Fighter)
            {
                Name = "Alpha-1 (Fighter)",
                Latitude = 47.4979,
                Longitude = 19.0402,
                Airspeed = 250.0,
                Capacity = 1
            };
            a1.FactionId = 1;
            a1.VisionRangeMeters = 5000.0;

            var v1 = new WarSim.Domain.Units.Vehicle(WarSim.Domain.VehicleType.Truck)
            {
                Name = "Bravo-2 (Truck)",
                Latitude = 47.50,
                Longitude = 19.05,
                GroundSpeed = 15.0,
                Crew = 2
            };
            v1.FactionId = 2;
            v1.VisionRangeMeters = 1000.0;

            var s1 = new WarSim.Domain.Units.Ship(WarSim.Domain.ShipType.Frigate)
            {
                Name = "Charlie-1 (Frigate)",
                Latitude = 47.48,
                Longitude = 19.03,
                SpeedKnots = 20.0,
                Crew = 120
            };
            s1.FactionId = 1;
            s1.VisionRangeMeters = 4000.0;

            var units = new List<Unit> { a1, v1, s1 };
            var projectiles = new List<Projectile>();

            var factions = new List<Faction>
            {
                new Faction { Id = 1, Name = "Blue", Color = "#0000FF", Allies = new List<int> { 1 } },
                new Faction { Id = 2, Name = "Red", Color = "#FF0000", Allies = new List<int> { 2 } }
            };

            _state = new WorldState(units, projectiles, factions, 0);
        }

        public WorldState GetSnapshot()
        {
            return Volatile.Read(ref _state);
        }

        public IReadOnlyList<Unit> GetUnits()
        {
            return GetSnapshot().Units;
        }

        public Unit? GetUnit(Guid id)
        {
            return GetSnapshot().Units.FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Atomically replace the current state with a new snapshot.
        /// </summary>
        public void UpdateState(WorldState newState)
        {
            Interlocked.Exchange(ref _state, newState);
        }

        /// <summary>
        /// Move a unit by creating a new snapshot with the updated unit position.
        /// </summary>
        public bool MoveUnit(Guid id, double lat, double lon)
        {
            var old = GetSnapshot();
            var units = old.Units.Select(CloneUnit).ToList();
            var idx = units.FindIndex(u => u.Id == id);
            if (idx < 0)
            {
                _logger.LogWarning("Attempted to move non-existing unit {UnitId}", id);
                return false;
            }

            var u = units[idx];
            u.Latitude = lat;
            u.Longitude = lon;
            u.Status = UnitStatus.Moving;

            var newState = new WorldState(units, old.Projectiles, old.Factions, old.Tick);
            UpdateState(newState);

            _logger.LogInformation("Unit {UnitId} moved to {Lat},{Lon} (snapshot)", id, lat, lon);
            return true;
        }

        private static Unit CloneUnit(Unit u)
        {
            return u switch
            {
                Aircraft a => SetId(new Aircraft(a.Type)
                {
                    Name = a.Name,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Heading = a.Heading,
                    Status = a.Status,
                    MaxAltitude = a.MaxAltitude,
                    Airspeed = a.Airspeed,
                    Capacity = a.Capacity
                }, a.Id),
                Helicopter h => SetId(new Helicopter()
                {
                    Name = h.Name,
                    Latitude = h.Latitude,
                    Longitude = h.Longitude,
                    Heading = h.Heading,
                    Status = h.Status,
                    Airspeed = h.Airspeed
                }, h.Id),
                Vehicle v => SetId(new Vehicle(v.Type)
                {
                    Name = v.Name,
                    Latitude = v.Latitude,
                    Longitude = v.Longitude,
                    Heading = v.Heading,
                    Status = v.Status,
                    GroundSpeed = v.GroundSpeed,
                    Crew = v.Crew
                }, v.Id),
                Infantry i => SetId(new Infantry()
                {
                    Name = i.Name,
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    Heading = i.Heading,
                    Status = i.Status,
                    Strength = i.Strength
                }, i.Id),
                Ship s => SetId(new Ship(s.Type)
                {
                    Name = s.Name,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Heading = s.Heading,
                    Status = s.Status,
                    SpeedKnots = s.SpeedKnots,
                    Crew = s.Crew
                }, s.Id),
                _ => throw new NotSupportedException($"Cloning unit type {u.GetType()} not supported")
            };

            static T SetId<T>(T obj, Guid id) where T : Unit
            {
                obj.Id = id;
                return obj;
            }
        }
    }

}
