using WarSim.Domain;

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

        public WorldStateService(
            ILogger<WorldStateService> logger,
            Simulation.IEntityProcessor entityProcessor,
            ScenarioService scenarioService)
        {
            _logger = logger;
            _entityProcessor = entityProcessor;

            try
            {
                _state = scenarioService.CreateCaucasusScenario();
                _logger.LogInformation(
                    "Initialized world state from caucasus-default scenario with {UnitCount} units",
                    _state.Units.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load caucasus-default scenario, falling back to empty world state");
                _state = new WorldState(new List<Unit>(), new List<WarSim.Domain.Projectiles.Projectile>(), new List<Faction>(), 0);
            }
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
