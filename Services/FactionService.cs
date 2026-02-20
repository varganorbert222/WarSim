using WarSim.Domain;

namespace WarSim.Services
{
    /// <summary>
    /// Manages factions and their alliance relationships. Updates the authoritative WorldState when changes occur.
    /// </summary>
    public class FactionService
    {
        private readonly WorldStateService _world;
        private readonly ILogger<FactionService> _logger;

        public FactionService(WorldStateService world, ILogger<FactionService> logger)
        {
            _world = world;
            _logger = logger;
        }

        public IReadOnlyList<Faction> GetFactions()
        {
            var snap = _world.GetSnapshot();
            return snap.Factions;
        }

        public bool UpdateAllies(int factionId, List<int> allies)
        {
            var snap = _world.GetSnapshot();
            var factions = snap.Factions.ToList();
            var idx = factions.FindIndex(f => f.Id == factionId);
            if (idx < 0)
            {
                return false;
            }

            // clone faction and set allies
            var f = factions[idx];
            var newFaction = new Faction
            {
                Id = f.Id,
                Name = f.Name,
                Color = f.Color,
                Allies = allies ?? new List<int>()
            };

            factions[idx] = newFaction;

            var newState = new WorldState(snap.Units, snap.Projectiles, factions, snap.Tick);
            _world.UpdateState(newState);
            _logger.LogInformation("Updated allies for faction {FactionId}", factionId);
            return true;
        }
    }
}
