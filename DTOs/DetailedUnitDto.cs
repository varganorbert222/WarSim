using System.Text.Json.Serialization;

namespace WarSim.DTOs
{
    /// <summary>
    /// Detailed unit information for frontend/Unity client visualization
    /// </summary>
    public class DetailedUnitDto
    {
        [JsonPropertyName("id")]
        public Guid Id
        {
            get; set;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("subcategory")]
        public string Subcategory { get; set; } = string.Empty;

        // Position and movement
        [JsonPropertyName("position")]
        public PositionDto Position { get; set; } = new();

        [JsonPropertyName("velocity")]
        public VelocityDto Velocity { get; set; } = new();

        [JsonPropertyName("directionVector")]
        public DirectionVectorDto DirectionVector { get; set; } = new();

        // Combat status
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("health")]
        public HealthDto Health { get; set; } = new();

        [JsonPropertyName("faction")]
        public FactionInfoDto Faction { get; set; } = new();

        // Vision and detection
        [JsonPropertyName("vision")]
        public VisionDto Vision { get; set; } = new();

        // Weapons and ammunition
        [JsonPropertyName("weapons")]
        public List<WeaponStatusDto> Weapons { get; set; } = new();

        [JsonPropertyName("ammo")]
        public AmmoStatusDto Ammo { get; set; } = new();

        // AI state (if AI controlled)
        [JsonPropertyName("aiState")]
        public AIStateDto? AIState
        {
            get; set;
        }

        // Unit specifications
        [JsonPropertyName("specifications")]
        public Dictionary<string, object> Specifications { get; set; } = new();

        // Base location
        [JsonPropertyName("baseLocation")]
        public string BaseLocation { get; set; } = string.Empty;
    }

    public class PositionDto
    {
        [JsonPropertyName("latitude")]
        public double Latitude
        {
            get; set;
        }

        [JsonPropertyName("longitude")]
        public double Longitude
        {
            get; set;
        }

        [JsonPropertyName("altitude")]
        public double? Altitude
        {
            get; set;
        }

        [JsonPropertyName("heading")]
        public double Heading
        {
            get; set;
        }
    }

    public class VelocityDto
    {
        [JsonPropertyName("speed")]
        public double Speed
        {
            get; set;
        }

        [JsonPropertyName("speedKnots")]
        public double? SpeedKnots
        {
            get; set;
        }

        [JsonPropertyName("groundSpeed")]
        public double? GroundSpeed
        {
            get; set;
        }

        [JsonPropertyName("airspeed")]
        public double? Airspeed
        {
            get; set;
        }

        [JsonPropertyName("velocityX")]
        public double VelocityX
        {
            get; set;
        }

        [JsonPropertyName("velocityY")]
        public double VelocityY
        {
            get; set;
        }

        [JsonPropertyName("climbRate")]
        public double? ClimbRate
        {
            get; set;
        }
    }

    public class DirectionVectorDto
    {
        [JsonPropertyName("heading")]
        public double Heading
        {
            get; set;
        }

        [JsonPropertyName("normalizedX")]
        public double NormalizedX
        {
            get; set;
        }

        [JsonPropertyName("normalizedY")]
        public double NormalizedY
        {
            get; set;
        }

        [JsonPropertyName("normalizedZ")]
        public double NormalizedZ
        {
            get; set;
        }
    }

    public class HealthDto
    {
        [JsonPropertyName("current")]
        public double Current
        {
            get; set;
        }

        [JsonPropertyName("maximum")]
        public double Maximum
        {
            get; set;
        }

        [JsonPropertyName("percentage")]
        public double Percentage
        {
            get; set;
        }

        [JsonPropertyName("armor")]
        public double Armor
        {
            get; set;
        }

        [JsonPropertyName("isDestroyed")]
        public bool IsDestroyed
        {
            get; set;
        }

        [JsonPropertyName("isDamaged")]
        public bool IsDamaged
        {
            get; set;
        }
    }

    public class FactionInfoDto
    {
        [JsonPropertyName("id")]
        public int Id
        {
            get; set;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("isAlly")]
        public bool IsAlly
        {
            get; set;
        }

        [JsonPropertyName("isEnemy")]
        public bool IsEnemy
        {
            get; set;
        }
    }

    public class VisionDto
    {
        [JsonPropertyName("rangeMeters")]
        public double RangeMeters
        {
            get; set;
        }

        [JsonPropertyName("detectionRangeMeters")]
        public double DetectionRangeMeters
        {
            get; set;
        }

        [JsonPropertyName("visibleEnemies")]
        public List<Guid> VisibleEnemies { get; set; } = new();

        [JsonPropertyName("visibleAllies")]
        public List<Guid> VisibleAllies { get; set; } = new();

        [JsonPropertyName("hasRadar")]
        public bool HasRadar
        {
            get; set;
        }

        [JsonPropertyName("radarRangeMeters")]
        public double? RadarRangeMeters
        {
            get; set;
        }
    }

    public class WeaponStatusDto
    {
        [JsonPropertyName("weaponId")]
        public string WeaponId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count
        {
            get; set;
        }

        [JsonPropertyName("projectileType")]
        public string ProjectileType { get; set; } = string.Empty;

        [JsonPropertyName("currentAmmo")]
        public int CurrentAmmo
        {
            get; set;
        }

        [JsonPropertyName("maxAmmo")]
        public int MaxAmmo
        {
            get; set;
        }

        [JsonPropertyName("currentMagazine")]
        public int CurrentMagazine
        {
            get; set;
        }

        [JsonPropertyName("magazineSize")]
        public int MagazineSize
        {
            get; set;
        }

        [JsonPropertyName("damage")]
        public double Damage
        {
            get; set;
        }

        [JsonPropertyName("rangeMeters")]
        public double RangeMeters
        {
            get; set;
        }

        [JsonPropertyName("rateOfFire")]
        public double RateOfFire
        {
            get; set;
        }

        [JsonPropertyName("isReloading")]
        public bool IsReloading
        {
            get; set;
        }

        [JsonPropertyName("canFire")]
        public bool CanFire
        {
            get; set;
        }
    }

    public class AmmoStatusDto
    {
        [JsonPropertyName("totalAmmoPercentage")]
        public double TotalAmmoPercentage
        {
            get; set;
        }

        [JsonPropertyName("totalRoundsRemaining")]
        public int TotalRoundsRemaining
        {
            get; set;
        }

        [JsonPropertyName("isLowAmmo")]
        public bool IsLowAmmo
        {
            get; set;
        }

        [JsonPropertyName("isOutOfAmmo")]
        public bool IsOutOfAmmo
        {
            get; set;
        }
    }

    public class AIStateDto
    {
        [JsonPropertyName("currentState")]
        public string CurrentState { get; set; } = string.Empty;

        [JsonPropertyName("lastState")]
        public string? LastState
        {
            get; set;
        }

        [JsonPropertyName("timeInState")]
        public double TimeInState
        {
            get; set;
        }

        [JsonPropertyName("currentTarget")]
        public TargetInfoDto? CurrentTarget
        {
            get; set;
        }

        [JsonPropertyName("behavior")]
        public AIBehaviorInfoDto Behavior { get; set; } = new();
    }

    public class TargetInfoDto
    {
        [JsonPropertyName("unitId")]
        public Guid UnitId
        {
            get; set;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("distance")]
        public double Distance
        {
            get; set;
        }

        [JsonPropertyName("heading")]
        public double Heading
        {
            get; set;
        }

        [JsonPropertyName("inRange")]
        public bool InRange
        {
            get; set;
        }
    }

    public class AIBehaviorInfoDto
    {
        [JsonPropertyName("unitType")]
        public string UnitType { get; set; } = string.Empty;

        [JsonPropertyName("aggressionLevel")]
        public double AggressionLevel
        {
            get; set;
        }

        [JsonPropertyName("engageRange")]
        public double EngageRange
        {
            get; set;
        }

        [JsonPropertyName("patrolRadius")]
        public double PatrolRadius
        {
            get; set;
        }
    }
}
