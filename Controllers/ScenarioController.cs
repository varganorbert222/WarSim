using Microsoft.AspNetCore.Mvc;
using WarSim.DTOs;
using WarSim.Services;

namespace WarSim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScenarioController : ControllerBase
    {
        private readonly ScenarioService _scenarioService;
        private readonly WorldStateService _worldState;
        private readonly MapService _mapService;
        private readonly ResponseCacheService _cache;

        public ScenarioController(ScenarioService scenarioService, WorldStateService worldState, MapService mapService, ResponseCacheService cache)
        {
            _scenarioService = scenarioService;
            _worldState = worldState;
            _mapService = mapService;
            _cache = cache;
        }

        /// <summary>
        /// Get current map definition (airbases, cities, naval zones, strategic points).
        /// Map is static and cached permanently.
        /// </summary>
        [HttpGet("map")]
        public ActionResult<MapDefinitionDto> GetMapDefinition()
        {
            var map = _cache.GetOrCreatePermanent("scenario_map", () =>
            {
                var mapData = _mapService.GetCurrentMap();
                if (mapData == null)
                {
                    throw new InvalidOperationException("No map loaded");
                }
                return mapData;
            });

            if (map == null)
            {
                return NotFound(new { error = "No map loaded" });
            }

            return Ok(map);
        }

        /// <summary>
        /// Load a mission from JSON and replace the current world state.
        /// Mission includes dynamic units and static structures.
        /// Map definition is loaded separately based on mapId.
        /// WARNING: This resets the entire simulation.
        /// </summary>
        [HttpPost("load")]
        public IActionResult LoadMission([FromBody] MissionDefinitionDto missionDto)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(missionDto);
                var newWorldState = _scenarioService.LoadMissionFromJson(json);
                _worldState.UpdateState(newWorldState);

                // Invalidate all caches when loading new mission
                _cache.ClearAll();

                WarSim.Logging.ConsoleColorLogger.Log("API", Microsoft.Extensions.Logging.LogLevel.Warning,
                    $"🔄 Mission loaded: '{missionDto.MissionName}' with {missionDto.Units.Count} units + {missionDto.StaticStructures.Count} structures");

                return Ok(new
                {
                    message = $"Mission '{missionDto.MissionName}' loaded successfully",
                    unitCount = missionDto.Units.Count,
                    structureCount = missionDto.StaticStructures.Count,
                    mapId = missionDto.MapId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Export current world state as a mission JSON definition.
        /// Cached based on world state tick.
        /// </summary>
        [HttpGet("export")]
        public ActionResult<MissionDefinitionDto> ExportMission()
        {
            var mission = _cache.GetOrCreate("scenario_export", () =>
            {
                var worldState = _worldState.GetSnapshot();
                var map = _mapService.GetCurrentMap();

                return new MissionDefinitionDto
                {
                    MissionName = "Exported Mission",
                    Description = "Mission exported from running simulation",
                    MapId = map?.Name.ToLowerInvariant().Replace(" ", "_") ?? "unknown",
                    Version = "1.0",
                    Factions = worldState.Factions.Select(f => new FactionDefinitionDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Color = f.Color,
                        Allies = f.Allies.ToList()
                    }).ToList(),
                    Units = worldState.Units
                        .Where(u => u is not Domain.Units.Structure)
                        .Select(u => MapUnitToDefinition(u))
                        .ToList(),
                    StaticStructures = worldState.Units
                        .OfType<Domain.Units.Structure>()
                        .Select(s => MapStructureToDefinition(s))
                        .ToList()
                };
            });

            return Ok(mission);
        }

        private UnitDefinitionDto MapUnitToDefinition(Domain.Unit u)
        {
            return new UnitDefinitionDto
            {
                Name = u.Name,
                Category = u.UnitCategory.ToString(),
                Subcategory = u.Subcategory,
                Latitude = u.Latitude,
                Longitude = u.Longitude,
                Heading = u.Heading,
                Speed = GetUnitSpeed(u),
                Status = u.Status.ToString(),
                FactionId = u.FactionId,
                Health = u.Health,
                VisionRangeMeters = u.VisionRangeMeters,
                Properties = ExtractUnitProperties(u)
            };
        }

        private StaticStructureDto MapStructureToDefinition(Domain.Units.Structure s)
        {
            return new StaticStructureDto
            {
                Name = s.Name,
                Category = s.UnitCategory.ToString(),
                Subcategory = s.Subcategory,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Heading = s.Heading,
                FactionId = s.FactionId,
                Health = s.Health,
                VisionRangeMeters = s.VisionRangeMeters
            };
        }

        private double GetUnitSpeed(Domain.Unit unit)
        {
            return unit switch
            {
                Domain.Units.LandUnit lu => lu.GroundSpeed ?? 0,
                Domain.Units.AirUnit au => au.Airspeed ?? 0,
                Domain.Units.SeaUnit su => (su.SpeedKnots ?? 0) * 0.514444,
                _ => 0
            };
        }

        private Dictionary<string, object> ExtractUnitProperties(Domain.Unit unit)
        {
            var props = new Dictionary<string, object>();

            switch (unit)
            {
                case Domain.Units.Aircraft a:
                    if (a.Capacity.HasValue)
                    {
                        props["capacity"] = a.Capacity.Value;
                    }
                    break;
                case Domain.Units.Infantry i:
                    if (i.Strength.HasValue)
                    {
                        props["strength"] = i.Strength.Value;
                    }
                    break;
                case Domain.Units.Vehicle v:
                    if (v.Crew.HasValue)
                    {
                        props["crew"] = v.Crew.Value;
                    }
                    break;
                case Domain.Units.Ship s:
                    if (s.Crew.HasValue)
                    {
                        props["crew"] = s.Crew.Value;
                    }
                    break;
            }

            return props;
        }
    }
}
