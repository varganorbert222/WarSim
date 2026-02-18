using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using WarSim.Domain;

namespace WarSim.Services
{
    public class WorldStateService
    {
        private readonly ConcurrentDictionary<Guid, Unit> _units = new();
        private readonly ILogger<WorldStateService> _logger;

        public WorldStateService(ILogger<WorldStateService> logger)
        {
            _logger = logger;

            // Demo egységek
            var u1 = new Unit
            {
                Name = "Alpha-1",
                Latitude = 47.4979,
                Longitude = 19.0402
            };

            var u2 = new Unit
            {
                Name = "Bravo-2",
                Latitude = 47.50,
                Longitude = 19.05
            };

            _units[u1.Id] = u1;
            _units[u2.Id] = u2;
        }

        public IReadOnlyList<Unit> GetUnits()
        {
            return _units.Values.ToList();
        }

        public Unit? GetUnit(Guid id)
        {
            return _units.TryGetValue(id, out var unit) ? unit : null;
        }

        /// <summary>
        /// Move a unit. Returns true if the unit existed and was updated.
        /// </summary>
        public bool MoveUnit(Guid id, double lat, double lon)
        {
            if (_units.TryGetValue(id, out var unit))
            {
                lock (unit)
                {
                    unit.Latitude = lat;
                    unit.Longitude = lon;
                    unit.Status = UnitStatus.Moving;
                }

                _logger.LogInformation("Unit {UnitId} moved to {Lat},{Lon}", id, lat, lon);
                return true;
            }

            _logger.LogWarning("Attempted to move non-existing unit {UnitId}", id);
            return false;
        }
    }

}
