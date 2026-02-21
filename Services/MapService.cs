using System.Text.Json;
using WarSim.DTOs;
using Microsoft.Extensions.Logging;

namespace WarSim.Services
{
    public class MapService
    {
        private readonly ILogger<MapService> _logger;
        private MapDefinitionDto? _currentMap;

        public MapService(ILogger<MapService> logger)
        {
            _logger = logger;
        }

        public MapDefinitionDto LoadMap(string mapId)
        {
            var path = Path.Combine("Data", "Maps", $"{mapId}-map.json");
            
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Map file not found: {path}");
            }

            var json = File.ReadAllText(path);
            var map = JsonSerializer.Deserialize<MapDefinitionDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (map == null)
            {
                throw new InvalidOperationException($"Failed to deserialize map: {mapId}");
            }

            _currentMap = map;
            _logger.LogInformation($"Loaded map '{map.Name}' with {map.Airbases.Count} airbases, {map.Cities.Count} cities");
            return map;
        }

        public MapDefinitionDto? GetCurrentMap() => _currentMap;

        public AirbaseDto? GetAirbase(string airbaseId)
        {
            return _currentMap?.Airbases.FirstOrDefault(a => a.Id.Equals(airbaseId, StringComparison.OrdinalIgnoreCase));
        }

        public CityDto? GetCity(string cityId)
        {
            return _currentMap?.Cities.FirstOrDefault(c => c.Id.Equals(cityId, StringComparison.OrdinalIgnoreCase));
        }

        public NavalZoneDto? GetNavalZone(string zoneId)
        {
            return _currentMap?.NavalZones.FirstOrDefault(n => n.Id.Equals(zoneId, StringComparison.OrdinalIgnoreCase));
        }

        public List<AirbaseDto> GetAirbasesByFaction(string faction)
        {
            return _currentMap?.Airbases
                .Where(a => a.ControlFaction.Equals(faction, StringComparison.OrdinalIgnoreCase))
                .ToList() ?? new List<AirbaseDto>();
        }

        public List<NavalZoneDto> GetNavalZonesWithRepair()
        {
            return _currentMap?.NavalZones
                .Where(n => n.Features.Contains("Repair"))
                .ToList() ?? new List<NavalZoneDto>();
        }
    }
}
