using System.Text.Json;
using WarSim.Domain;
using WarSim.Domain.Projectiles;
using WarSim.Domain.Units;
using WarSim.DTOs;
using Microsoft.Extensions.Logging;

namespace WarSim.Services
{
    /// <summary>
    /// Service for creating and loading scenario setups from JSON definitions.
    /// Supports mission editor integration via JSON import/export.
    /// </summary>
    public class ScenarioService
    {
        private readonly ILogger<ScenarioService> _logger;
        private readonly WeaponConfigService _weaponConfig;
        private readonly MapService _mapService;

        public ScenarioService(ILogger<ScenarioService> logger, WeaponConfigService weaponConfig, MapService mapService)
        {
            _logger = logger;
            _weaponConfig = weaponConfig;
            _mapService = mapService;
        }

        public WorldState LoadMissionFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Mission file not found: {filePath}");
            }

            var json = File.ReadAllText(filePath);
            return LoadMissionFromJson(json);
        }

        public WorldState LoadMissionFromJson(string json)
        {
            var missionDto = JsonSerializer.Deserialize<MissionDefinitionDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (missionDto == null)
            {
                throw new InvalidOperationException("Failed to deserialize mission JSON");
            }

            // Load the map
            _mapService.LoadMap(missionDto.MapId);

            return BuildWorldStateFromMission(missionDto);
        }

        private WorldState BuildWorldStateFromMission(MissionDefinitionDto mission)
        {
            var factions = mission.Factions.Select(f => new Faction
            {
                Id = f.Id,
                Name = f.Name,
                Color = f.Color,
                Allies = f.Allies.ToList()
            }).ToList();

            var units = new List<Unit>();
            
            // Add dynamic units
            units.AddRange(mission.Units.Select(u => CreateUnitFromDto(u)));
            
            // Add static structures
            units.AddRange(mission.StaticStructures.Select(s => CreateStructureFromDto(s)));

            // Add static structures from map
            var map = _mapService.GetCurrentMap();
            if (map != null)
            {
                units.AddRange(map.AirbaseStructures.Select(s => CreateStructureFromMapDto(s)));
                units.AddRange(map.CityStructures.Select(s => CreateStructureFromMapDto(s)));
            }

            var projectiles = new List<Projectile>();

            _logger.LogInformation($"Loaded mission '{mission.MissionName}' with {mission.Units.Count} dynamic units + {units.Count - mission.Units.Count} static structures");

            return new WorldState(units, projectiles, factions, 0);
        }

        private Structure CreateStructureFromMapDto(MapStructureDto dto)
        {
            if (!Enum.TryParse<StructureSubcategory>(dto.Subcategory, true, out var subcat))
            {
                subcat = StructureSubcategory.MilitaryBuilding;
            }

            var structure = new Structure(subcat)
            {
                Name = dto.Name,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Heading = dto.Heading,
                Status = UnitStatus.Idle,
                FactionId = dto.FactionId,
                Health = dto.Health,
                VisionRangeMeters = dto.VisionRangeMeters
            };

            return structure;
        }

        private Structure CreateStructureFromDto(StaticStructureDto dto)
        {
            if (!Enum.TryParse<StructureSubcategory>(dto.Subcategory, true, out var subcat))
            {
                subcat = StructureSubcategory.MilitaryBuilding;
            }

            var structure = new Structure(subcat)
            {
                Name = dto.Name,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Heading = dto.Heading,
                Status = UnitStatus.Idle,
                FactionId = dto.FactionId,
                Health = dto.Health,
                VisionRangeMeters = dto.VisionRangeMeters
            };

            return structure;
        }

        public ScenarioDefinitionDto ExportScenarioToDto(WorldState worldState)
        {
            var dto = new ScenarioDefinitionDto
            {
                Name = "Exported Scenario",
                Description = "Scenario exported from running simulation",
                Factions = worldState.Factions.Select(f => new FactionDefinitionDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Color = f.Color,
                    Allies = f.Allies.ToList()
                }).ToList(),
                Units = worldState.Units.Select(u => new UnitDefinitionDto
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
                }).ToList()
            };

            return dto;
        }

        private WorldState BuildWorldStateFromDto(ScenarioDefinitionDto dto)
        {
            var factions = dto.Factions.Select(f => new Faction
            {
                Id = f.Id,
                Name = f.Name,
                Color = f.Color,
                Allies = f.Allies.ToList()
            }).ToList();

            var units = dto.Units.Select(u => CreateUnitFromDto(u)).ToList();
            var projectiles = new List<Projectile>();

            _logger.LogInformation($"Loaded scenario '{dto.Name}' with {units.Count} units and {factions.Count} factions");

            return new WorldState(units, projectiles, factions, 0);
        }

        private Unit CreateUnitFromDto(UnitDefinitionDto dto)
        {
            if (!Enum.TryParse<UnitCategory>(dto.Category, true, out var category))
            {
                throw new InvalidOperationException($"Invalid unit category: {dto.Category}");
            }

            if (!Enum.TryParse<UnitStatus>(dto.Status, true, out var status))
            {
                status = UnitStatus.Idle;
            }

            Unit unit = category switch
            {
                UnitCategory.AIRPLANE => CreateAircraft(dto),
                UnitCategory.HELICOPTER => CreateHelicopter(dto),
                UnitCategory.GROUND_UNIT => CreateGroundUnit(dto),
                UnitCategory.SHIP => CreateShip(dto),
                UnitCategory.STRUCTURE => CreateStructure(dto),
                _ => throw new NotSupportedException($"Unsupported unit category: {category}")
            };

            // Set common properties
            unit.Name = dto.Name;
            unit.Latitude = dto.Latitude;
            unit.Longitude = dto.Longitude;
            unit.Heading = dto.Heading;
            unit.Status = status;
            unit.FactionId = dto.FactionId;
            unit.Health = dto.Health;
            unit.VisionRangeMeters = dto.VisionRangeMeters;

            // Set speed based on unit type
            SetUnitSpeed(unit, dto.Speed);

            // Initialize weapon slots from config
            InitializeWeapons(unit);

            return unit;
        }

        private void InitializeWeapons(Unit unit)
        {
            var loadout = _weaponConfig.GetLoadoutForUnit(unit.UnitCategory.ToString(), unit.Subcategory);
            if (loadout == null) return;

            foreach (var weaponSlot in loadout.Weapons)
            {
                var weapon = _weaponConfig.GetWeapon(weaponSlot.WeaponId);
                if (weapon == null) continue;

                var slot = new Domain.Weapons.WeaponSlot
                {
                    WeaponId = weapon.Id,
                    Count = weaponSlot.Count,
                    CurrentAmmo = weapon.TotalAmmo,
                    CurrentMagazine = weapon.MagazineSize,
                    LastFireTime = 0,
                    IsReloading = false,
                    ReloadStartTime = 0
                };

                unit.WeaponSlots.Add(slot);
            }

            if (unit.WeaponSlots.Any())
            {
                _logger.LogDebug($"Initialized {unit.WeaponSlots.Count} weapon slots for {unit.Name}");
            }
        }

        private Aircraft CreateAircraft(UnitDefinitionDto dto)
        {
            if (!Enum.TryParse<AirplaneSubcategory>(dto.Subcategory, true, out var subcat))
            {
                subcat = AirplaneSubcategory.Fighter;
            }

            var aircraft = new Aircraft(subcat);

            if (dto.Properties.TryGetValue("capacity", out var capacityObj) && capacityObj != null)
            {
                aircraft.Capacity = Convert.ToInt32(capacityObj);
            }

            return aircraft;
        }

        private Helicopter CreateHelicopter(UnitDefinitionDto dto)
        {
            if (!Enum.TryParse<HelicopterSubcategory>(dto.Subcategory, true, out var subcat))
            {
                subcat = HelicopterSubcategory.UtilityHelicopter;
            }

            return new Helicopter(subcat);
        }

        private Unit CreateGroundUnit(UnitDefinitionDto dto)
        {
            if (dto.Subcategory.Equals("Infantry", StringComparison.OrdinalIgnoreCase))
            {
                var infantry = new Infantry();
                if (dto.Properties.TryGetValue("strength", out var strengthObj) && strengthObj != null)
                {
                    infantry.Strength = Convert.ToInt32(strengthObj);
                }
                return infantry;
            }

            if (!Enum.TryParse<GroundUnitSubcategory>(dto.Subcategory, true, out var subcat))
            {
                subcat = GroundUnitSubcategory.ReconVehicle;
            }

            var vehicle = new Vehicle(subcat);
            if (dto.Properties.TryGetValue("crew", out var crewObj) && crewObj != null)
            {
                vehicle.Crew = Convert.ToInt32(crewObj);
            }

            return vehicle;
        }

        private Ship CreateShip(UnitDefinitionDto dto)
        {
            if (!Enum.TryParse<ShipSubcategory>(dto.Subcategory, true, out var subcat))
            {
                subcat = ShipSubcategory.Frigate;
            }

            var ship = new Ship(subcat);
            if (dto.Properties.TryGetValue("crew", out var crewObj) && crewObj != null)
            {
                ship.Crew = Convert.ToInt32(crewObj);
            }

            return ship;
        }

        private Structure CreateStructure(UnitDefinitionDto dto)
        {
            if (!Enum.TryParse<StructureSubcategory>(dto.Subcategory, true, out var subcat))
            {
                subcat = StructureSubcategory.MilitaryBuilding;
            }

            return new Structure(subcat);
        }

        private void SetUnitSpeed(Unit unit, double speed)
        {
            switch (unit)
            {
                case LandUnit lu:
                    lu.GroundSpeed = speed;
                    break;
                case AirUnit au:
                    au.Airspeed = speed;
                    break;
                case SeaUnit su:
                    // JSON has m/s, convert to knots
                    su.SpeedKnots = speed / 0.514444;
                    break;
            }
        }

        private double GetUnitSpeed(Unit unit)
        {
            return unit switch
            {
                LandUnit lu => lu.GroundSpeed ?? 0,
                AirUnit au => au.Airspeed ?? 0,
                SeaUnit su => (su.SpeedKnots ?? 0) * 0.514444, // knots to m/s
                _ => 0
            };
        }

        private Dictionary<string, object> ExtractUnitProperties(Unit unit)
        {
            var props = new Dictionary<string, object>();

            switch (unit)
            {
                case Aircraft a:
                    if (a.Capacity.HasValue)
                    {
                        props["capacity"] = a.Capacity.Value;
                    }

                    break;
                case Infantry i:
                    if (i.Strength.HasValue)
                    {
                        props["strength"] = i.Strength.Value;
                    }

                    break;
                case Vehicle v:
                    if (v.Crew.HasValue)
                    {
                        props["crew"] = v.Crew.Value;
                    }

                    break;
                case Ship s:
                    if (s.Crew.HasValue)
                    {
                        props["crew"] = s.Crew.Value;
                    }

                    break;
            }

            return props;
        }

        /// <summary>
        /// Loads default Caucasus mission from mission JSON file
        /// </summary>
        public WorldState CreateCaucasusScenario()
        {
            var missionPath = Path.Combine("Data", "Missions", "caucasus-default.json");
            return LoadMissionFromFile(missionPath);
        }

        [Obsolete("Use LoadMissionFromFile instead")]
        public WorldState LoadScenarioFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Scenario file not found: {filePath}");
            }

            var json = File.ReadAllText(filePath);
            return LoadScenarioFromJson(json);
        }

        [Obsolete("Use LoadMissionFromJson instead")]
        public WorldState LoadScenarioFromJson(string json)
        {
            var scenarioDto = JsonSerializer.Deserialize<ScenarioDefinitionDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (scenarioDto == null)
            {
                throw new InvalidOperationException("Failed to deserialize scenario JSON");
            }

            return BuildWorldStateFromDto(scenarioDto);
        }
    }
}
