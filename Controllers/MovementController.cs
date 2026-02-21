using Microsoft.AspNetCore.Mvc;
using WarSim.Domain.Projectiles;
using WarSim.Domain.Units;
using WarSim.DTOs;
using WarSim.Services;

namespace WarSim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementController : ControllerBase
    {
        private readonly WorldStateService _world;
        private readonly ResponseCacheService _cache;

        public MovementController(WorldStateService world, ResponseCacheService cache)
        {
            _world = world;
            _cache = cache;
        }

        /// <summary>
        /// Get optimized movement snapshot for client-side interpolation and prediction.
        /// Returns normalized speed values (m/s) and essential position/heading data.
        /// Uses caching to avoid rebuilding the same snapshot for the same tick.
        /// </summary>
        [HttpGet("snapshot")]
        public ActionResult<MovementSnapshotDto> GetSnapshot()
        {
            var dto = _cache.GetOrCreate("movement_snapshot", () =>
            {
                var snap = _world.GetSnapshot();
                return new MovementSnapshotDto
                {
                    Tick = snap.Tick,
                    Timestamp = DateTime.UtcNow,
                    Units = snap.Units.Select(MapUnitToMovement).ToList(),
                    Projectiles = snap.Projectiles.Select(MapProjectileToMovement).ToList()
                };
            });

            // Update timestamp for each request
            dto.Timestamp = DateTime.UtcNow;

            return Ok(dto);
        }

        private static UnitMovementDto MapUnitToMovement(Domain.Unit u)
        {
            double speedMps = 0.0;
            double? altitude = null;

            switch (u)
            {
                case LandUnit lu when lu.GroundSpeed.HasValue:
                    speedMps = lu.GroundSpeed.Value;
                    break;
                case AirUnit au when au.Airspeed.HasValue:
                    speedMps = au.Airspeed.Value;
                    altitude = au.MaxAltitude ?? 0;
                    break;
                case SeaUnit su when su.SpeedKnots.HasValue:
                    speedMps = su.SpeedKnots.Value * 0.514444; // knots to m/s
                    break;
            }

            // Calculate direction vector
            var radians = u.Heading * Math.PI / 180.0;
            var directionX = Math.Sin(radians);
            var directionY = Math.Cos(radians);

            return new UnitMovementDto
            {
                Id = u.Id,
                Name = u.Name,
                Category = u.UnitCategory.ToString(),
                Subcategory = u.Subcategory,
                Latitude = u.Latitude,
                Longitude = u.Longitude,
                Altitude = altitude,
                Heading = u.Heading,
                SpeedMps = speedMps,
                Status = u.Status.ToString(),
                FactionId = u.FactionId,
                Health = u.Health,
                VisionRange = u.VisionRangeMeters,
                DirectionX = directionX,
                DirectionY = directionY,
                AmmoPercentage = u.GetAmmoPercent()
            };
        }

        private static ProjectileMovementDto MapProjectileToMovement(Projectile p)
        {
            var type = p switch
            {
                Bullet => "Bullet",
                Shell => "Shell",
                Missile => "Missile",
                _ => "Unknown"
            };

            return new ProjectileMovementDto
            {
                Id = p.Id,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Heading = p.Heading,
                SpeedMps = p.Speed,
                Type = type,
                OwnerUnitId = p.OwnerUnitId
            };
        }
    }
}
