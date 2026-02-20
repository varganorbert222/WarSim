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

        public MovementController(WorldStateService world)
        {
            _world = world;
        }

        /// <summary>
        /// Get optimized movement snapshot for client-side interpolation and prediction.
        /// Returns normalized speed values (m/s) and essential position/heading data.
        /// </summary>
        [HttpGet("snapshot")]
        public ActionResult<MovementSnapshotDto> GetSnapshot()
        {
            var snap = _world.GetSnapshot();
            var dto = new MovementSnapshotDto
            {
                Tick = snap.Tick,
                Timestamp = DateTime.UtcNow,
                Units = snap.Units.Select(MapUnitToMovement).ToList(),
                Projectiles = snap.Projectiles.Select(MapProjectileToMovement).ToList()
            };

            return Ok(dto);
        }

        private static UnitMovementDto MapUnitToMovement(Domain.Unit u)
        {
            double speedMps = 0.0;
            switch (u)
            {
                case LandUnit lu when lu.GroundSpeed.HasValue:
                    speedMps = lu.GroundSpeed.Value;
                    break;
                case AirUnit au when au.Airspeed.HasValue:
                    speedMps = au.Airspeed.Value;
                    break;
                case SeaUnit su when su.SpeedKnots.HasValue:
                    speedMps = su.SpeedKnots.Value * 0.514444; // knots to m/s
                    break;
            }

            return new UnitMovementDto
            {
                Id = u.Id,
                Name = u.Name,
                Latitude = u.Latitude,
                Longitude = u.Longitude,
                Heading = u.Heading,
                SpeedMps = speedMps,
                Status = u.Status.ToString(),
                FactionId = u.FactionId
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
