using System.Text.Json;
using Microsoft.Extensions.Logging;
using WarSim.DTOs;

namespace WarSim.Services
{
    public class MapService
    {
        private readonly ILogger<MapService> _logger;
        private readonly Dictionary<string, MapDefinitionDto> _maps = new();

        public MapService(ILogger<MapService> logger)
        {
            _logger = logger;
            LoadMaps();
        }

        private void LoadMaps()
        {
            try
            {
                var mapsPath = Path.Combine("Data", "Maps");
                if (!Directory.Exists(mapsPath))
                {
                    _logger.LogWarning("Maps directory not found: {Path}", mapsPath);
                    return;
                }

                var mapFiles = Directory.GetFiles(mapsPath, "*-map.json");
                foreach (var mapFile in mapFiles)
                {
                    var json = File.ReadAllText(mapFile);
                    var map = JsonSerializer.Deserialize<MapDefinitionDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (map != null)
                    {
                        var mapId = Path.GetFileNameWithoutExtension(mapFile).Replace("-map", "");
                        _maps[mapId] = map;
                        _logger.LogInformation($"Loaded map '{map.Name}' with ID '{mapId}': {map.Airbases.Count} airbases, {map.Cities.Count} cities, {map.StaticStructures.Count} structures");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load maps");
            }
        }

        public MapDefinitionDto? GetMap(string mapId)
        {
            return _maps.TryGetValue(mapId, out var map) ? map : null;
        }

        public List<string> GetAvailableMapIds()
        {
            return _maps.Keys.ToList();
        }

        public List<MapDefinitionDto> GetAllMaps()
        {
            return _maps.Values.ToList();
        }
    }
}
