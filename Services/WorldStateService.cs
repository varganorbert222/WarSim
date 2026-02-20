using WarSim.Domain;
using WarSim.Domain.Projectiles;

namespace WarSim.Services
{
    /// <summary>
    /// Holds and provides access to the authoritative world state. Uses snapshot-update-swap semantics.
    /// </summary>
    public class WorldStateService
    {
        private WorldState _state;
        private readonly ILogger<WorldStateService> _logger;
        private readonly Simulation.IEntityProcessor _entityProcessor;

        public WorldStateService(ILogger<WorldStateService> _logger, Simulation.IEntityProcessor entityProcessor)
        {
            this._logger = _logger;
            _entityProcessor = entityProcessor;

            // Demo units using the new category hierarchy
            var a1 = new WarSim.Domain.Units.Aircraft(WarSim.Domain.AirplaneSubcategory.Fighter)
            {
                Name = "Alpha-1 (Fighter)",
                Latitude = 47.4979,
                Longitude = 19.0402,
                Airspeed = 250.0,
                Capacity = 1,
                FactionId = 1,
                VisionRangeMeters = 5000.0
            };

            var h1 = new WarSim.Domain.Units.Helicopter(WarSim.Domain.HelicopterSubcategory.AttackHelicopter)
            {
                Name = "Delta-1 (Attack Heli)",
                Latitude = 47.49,
                Longitude = 19.04,
                Airspeed = 150.0,
                FactionId = 1,
                VisionRangeMeters = 3000.0
            };

            var v1 = new WarSim.Domain.Units.Vehicle(WarSim.Domain.GroundUnitSubcategory.MainBattleTank)
            {
                Name = "Bravo-2 (MBT)",
                Latitude = 47.50,
                Longitude = 19.05,
                GroundSpeed = 15.0,
                Crew = 4,
                FactionId = 2,
                VisionRangeMeters = 2000.0
            };

            var i1 = new WarSim.Domain.Units.Infantry()
            {
                Name = "Echo-1 (Infantry)",
                Latitude = 47.505,
                Longitude = 19.055,
                GroundSpeed = 2.0,
                Strength = 10,
                FactionId = 2,
                VisionRangeMeters = 500.0
            };

            var s1 = new WarSim.Domain.Units.Ship(WarSim.Domain.ShipSubcategory.Frigate)
            {
                Name = "Charlie-1 (Frigate)",
                Latitude = 47.48,
                Longitude = 19.03,
                SpeedKnots = 20.0,
                Crew = 120,
                FactionId = 1,
                VisionRangeMeters = 4000.0
            };

            var st1 = new WarSim.Domain.Units.Structure(WarSim.Domain.StructureSubcategory.RadarTower)
            {
                Name = "Foxtrot-1 (Radar)",
                Latitude = 47.51,
                Longitude = 19.06,
                FactionId = 1,
                VisionRangeMeters = 10000.0,
                Health = 200.0
            };

            var units = new List<Unit> { a1, h1, v1, i1, s1, st1 };

            var projectiles = new List<Projectile>
            {
                new Bullet
                {
                    Latitude = 47.4985,
                    Longitude = 19.041,
                    Speed = 900,
                    Heading = 45,
                    Damage = 10,
                    OwnerUnitId = a1.Id,
                },
                new Shell
                {
                    Latitude = 47.499,
                    Longitude = 19.042,
                    Speed = 400,
                    Heading = 180,
                    Damage = 50,
                    OwnerUnitId = v1.Id,
                }
            };

            var factions = new List<Faction>
            {
                new() { Id = 1, Name = "Blue", Color = "#0000FF", Allies = new List<int> { 1 } },
                new() { Id = 2, Name = "Red", Color = "#FF0000", Allies = new List<int> { 2 } }
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
            _ = Interlocked.Exchange(ref _state, newState);
        }

        /// <summary>
        /// Move a unit by creating a new snapshot with the updated unit position.
        /// </summary>
        public bool MoveUnit(Guid id, double lat, double lon)
        {
            var old = GetSnapshot();
            var units = old.Units.Select(u => _entityProcessor.CloneUnit(u)).ToList();
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
            WarSim.Logging.ConsoleColorLogger.Log("WorldState", Microsoft.Extensions.Logging.LogLevel.Information, $"Unit {id} moved to {lat:F6},{lon:F6}");
            return true;
        }
    }

}
