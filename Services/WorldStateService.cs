using WarSim.Domain;

namespace WarSim.Services
{
    public class WorldStateService
    {
        private readonly List<Unit> _units = new();

        public WorldStateService()
        {
            // Demo egységek
            _units.Add(new Unit
            {
                Name = "Alpha-1",
                Latitude = 47.4979,
                Longitude = 19.0402
            });

            _units.Add(new Unit
            {
                Name = "Bravo-2",
                Latitude = 47.50,
                Longitude = 19.05
            });
        }

        public IReadOnlyList<Unit> GetUnits()
        {
            return _units;
        }

        public Unit? GetUnit(Guid id)
        {
            return _units.FirstOrDefault(u => u.Id == id);
        }

        public void MoveUnit(Guid id, double lat, double lon)
        {
            var unit = GetUnit(id);
            if (unit is null)
            {
                return;
            }

            unit.Latitude = lat;
            unit.Longitude = lon;
            unit.Status = UnitStatus.Moving;
        }
    }

}