using WarSim.Domain;
using WarSim.DTOs;
using WarSim.Services;
using Microsoft.Extensions.Logging;

namespace WarSim.Services
{
    public class UnitInfoService
    {
        private readonly ILogger<UnitInfoService> _logger;
        private readonly WeaponConfigService _weaponConfig;
        private readonly DamageConfigService _damageConfig;

        public UnitInfoService(
            ILogger<UnitInfoService> logger,
            WeaponConfigService weaponConfig,
            DamageConfigService damageConfig)
        {
            _logger = logger;
            _weaponConfig = weaponConfig;
            _damageConfig = damageConfig;
        }

        public DetailedUnitDto CreateDetailedUnitDto(Unit unit, WorldState worldState, int? requestorFactionId = null)
        {
            var faction = worldState.Factions.FirstOrDefault(f => f.Id == unit.FactionId);
            var directionVector = CalculateDirectionVector(unit.Heading);

            var dto = new DetailedUnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                Category = unit.UnitCategory.ToString(),
                Subcategory = unit.Subcategory,
                Position = new PositionDto
                {
                    Latitude = unit.Latitude,
                    Longitude = unit.Longitude,
                    Altitude = GetAltitude(unit),
                    Heading = unit.Heading
                },
                Velocity = CreateVelocityDto(unit),
                DirectionVector = directionVector,
                Status = unit.Status.ToString(),
                Health = CreateHealthDto(unit),
                Faction = CreateFactionDto(faction, unit.FactionId, requestorFactionId, worldState),
                Vision = CreateVisionDto(unit, worldState),
                Weapons = CreateWeaponStatusList(unit),
                Ammo = CreateAmmoStatusDto(unit),
                Specifications = ExtractSpecifications(unit)
            };

            return dto;
        }

        private DirectionVectorDto CalculateDirectionVector(double heading)
        {
            var radians = heading * Math.PI / 180.0;
            return new DirectionVectorDto
            {
                Heading = heading,
                NormalizedX = Math.Sin(radians),
                NormalizedY = Math.Cos(radians),
                NormalizedZ = 0 // 2D for now, 3D requires pitch
            };
        }

        private double? GetAltitude(Unit unit)
        {
            if (unit is Domain.Units.AirUnit airUnit)
            {
                return airUnit.MaxAltitude ?? 0;
            }
            return null;
        }

        private VelocityDto CreateVelocityDto(Unit unit)
        {
            var dto = new VelocityDto();

            switch (unit)
            {
                case Domain.Units.AirUnit au:
                    dto.Airspeed = au.Airspeed ?? 0;
                    dto.Speed = au.Airspeed ?? 0;
                    dto.ClimbRate = 0; // TODO: implement climb rate
                    break;
                case Domain.Units.LandUnit lu:
                    dto.GroundSpeed = lu.GroundSpeed ?? 0;
                    dto.Speed = lu.GroundSpeed ?? 0;
                    break;
                case Domain.Units.SeaUnit su:
                    dto.SpeedKnots = su.SpeedKnots ?? 0;
                    dto.Speed = (su.SpeedKnots ?? 0) * 0.514444; // knots to m/s
                    break;
                default:
                    dto.Speed = 0;
                    break;
            }

            // Calculate velocity components
            var radians = unit.Heading * Math.PI / 180.0;
            dto.VelocityX = dto.Speed * Math.Sin(radians);
            dto.VelocityY = dto.Speed * Math.Cos(radians);

            return dto;
        }

        private HealthDto CreateHealthDto(Unit unit)
        {
            var maxHealth = 100.0; // Default, should come from unit type config
            var armor = _damageConfig.GetArmorValue(unit);

            return new HealthDto
            {
                Current = unit.Health,
                Maximum = maxHealth,
                Percentage = unit.Health / maxHealth,
                Armor = armor,
                IsDestroyed = unit.Status == UnitStatus.Destroyed,
                IsDamaged = unit.Health < maxHealth && unit.Health > 0
            };
        }

        private FactionInfoDto CreateFactionDto(Faction? faction, int unitFactionId, int? requestorFactionId, WorldState worldState)
        {
            var isAlly = false;
            var isEnemy = false;

            if (requestorFactionId.HasValue)
            {
                isAlly = unitFactionId == requestorFactionId.Value;
                var requestorFaction = worldState.Factions.FirstOrDefault(f => f.Id == requestorFactionId.Value);
                isEnemy = requestorFaction != null && !requestorFaction.Allies.Contains(unitFactionId);
            }

            return new FactionInfoDto
            {
                Id = unitFactionId,
                Name = faction?.Name ?? "Unknown",
                Color = faction?.Color ?? "#808080",
                IsAlly = isAlly,
                IsEnemy = isEnemy
            };
        }

        private VisionDto CreateVisionDto(Unit unit, WorldState worldState)
        {
            var visibleEnemies = new List<Guid>();
            var visibleAllies = new List<Guid>();

            foreach (var other in worldState.Units)
            {
                if (other.Id == unit.Id || other.Status == UnitStatus.Destroyed)
                    continue;

                var distance = DistanceMeters(unit, other);
                if (distance <= unit.VisionRangeMeters)
                {
                    if (other.FactionId == unit.FactionId)
                        visibleAllies.Add(other.Id);
                    else
                        visibleEnemies.Add(other.Id);
                }
            }

            return new VisionDto
            {
                RangeMeters = unit.VisionRangeMeters,
                DetectionRangeMeters = unit.VisionRangeMeters, // Same for now
                VisibleEnemies = visibleEnemies,
                VisibleAllies = visibleAllies,
                HasRadar = unit.Subcategory.Contains("Radar") || unit.Subcategory == "AWACS",
                RadarRangeMeters = unit.Subcategory == "AWACS" ? unit.VisionRangeMeters : null
            };
        }

        private List<WeaponStatusDto> CreateWeaponStatusList(Unit unit)
        {
            var weaponStatusList = new List<WeaponStatusDto>();

            foreach (var slot in unit.WeaponSlots)
            {
                var weapon = _weaponConfig.GetWeapon(slot.WeaponId);
                if (weapon == null) continue;

                weaponStatusList.Add(new WeaponStatusDto
                {
                    WeaponId = weapon.Id,
                    Name = weapon.Name,
                    Count = slot.Count,
                    ProjectileType = weapon.ProjectileType,
                    CurrentAmmo = slot.CurrentAmmo,
                    MaxAmmo = weapon.TotalAmmo,
                    CurrentMagazine = slot.CurrentMagazine,
                    MagazineSize = weapon.MagazineSize,
                    Damage = weapon.Damage,
                    RangeMeters = weapon.RangeMeters,
                    RateOfFire = weapon.RateOfFire,
                    IsReloading = slot.IsReloading,
                    CanFire = slot.CurrentMagazine > 0 && !slot.IsReloading
                });
            }

            return weaponStatusList;
        }

        private AmmoStatusDto CreateAmmoStatusDto(Unit unit)
        {
            var totalMax = unit.WeaponSlots.Sum(w => w.CurrentAmmo + (w.CurrentMagazine * 10)); // rough estimate
            var totalCurrent = unit.WeaponSlots.Sum(w => w.CurrentAmmo);
            var percentage = totalMax > 0 ? (double)totalCurrent / totalMax : 0;

            return new AmmoStatusDto
            {
                TotalAmmoPercentage = percentage,
                TotalRoundsRemaining = totalCurrent,
                IsLowAmmo = percentage < 0.3,
                IsOutOfAmmo = totalCurrent == 0
            };
        }

        private Dictionary<string, object> ExtractSpecifications(Unit unit)
        {
            var specs = new Dictionary<string, object>();

            switch (unit)
            {
                case Domain.Units.Aircraft a:
                    specs["maxAltitude"] = a.MaxAltitude ?? 0;
                    specs["capacity"] = a.Capacity ?? 0;
                    break;
                case Domain.Units.Helicopter h:
                    break;
                case Domain.Units.Infantry i:
                    specs["strength"] = i.Strength ?? 0;
                    break;
                case Domain.Units.Vehicle v:
                    specs["crew"] = v.Crew ?? 0;
                    break;
                case Domain.Units.Ship s:
                    specs["crew"] = s.Crew ?? 0;
                    break;
            }

            return specs;
        }

        private double DistanceMeters(Unit u1, Unit u2)
        {
            const double metersPerDegLat = 111320.0;
            var dy = (u2.Latitude - u1.Latitude) * metersPerDegLat;
            var avgLat = (u1.Latitude + u2.Latitude) / 2.0 * Math.PI / 180.0;
            var metersPerDegLon = metersPerDegLat * Math.Cos(avgLat);
            var dx = (u2.Longitude - u1.Longitude) * metersPerDegLon;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
