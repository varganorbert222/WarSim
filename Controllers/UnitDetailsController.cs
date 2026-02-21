using Microsoft.AspNetCore.Mvc;
using WarSim.DTOs;
using WarSim.Services;

namespace WarSim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitDetailsController : ControllerBase
    {
        private readonly WorldStateService _worldState;
        private readonly UnitInfoService _unitInfo;
        private readonly ResponseCacheService _cache;

        public UnitDetailsController(WorldStateService worldState, UnitInfoService unitInfo, ResponseCacheService cache)
        {
            _worldState = worldState;
            _unitInfo = unitInfo;
            _cache = cache;
        }

        /// <summary>
        /// Get detailed information about a specific unit
        /// </summary>
        [HttpGet("{unitId}")]
        public ActionResult<DetailedUnitDto> GetUnitDetails(Guid unitId, [FromQuery] int? requestorFactionId = null)
        {
            var snapshot = _worldState.GetSnapshot();
            var cacheKey = $"unit_{unitId}_{requestorFactionId}";

            var detailedInfo = _cache.GetOrCreate(cacheKey, snapshot.Tick, () =>
            {
                var unit = snapshot.Units.FirstOrDefault(u => u.Id == unitId);
                if (unit == null)
                {
                    return null;
                }
                return _unitInfo.CreateDetailedUnitDto(unit, snapshot, requestorFactionId);
            });

            if (detailedInfo == null)
            {
                return NotFound(new { error = $"Unit with ID {unitId} not found" });
            }

            return Ok(detailedInfo);
        }

        /// <summary>
        /// Get detailed information about all units (optionally filtered by faction)
        /// </summary>
        [HttpGet]
        public ActionResult<List<DetailedUnitDto>> GetAllUnitsDetails([FromQuery] int? factionId = null, [FromQuery] int? requestorFactionId = null)
        {
            var snapshot = _worldState.GetSnapshot();
            var cacheKey = $"all_{factionId}_{requestorFactionId}";

            var detailedInfoList = _cache.GetOrCreate(cacheKey, snapshot.Tick, () =>
            {
                var units = snapshot.Units.AsEnumerable();

                if (factionId.HasValue)
                {
                    units = units.Where(u => u.FactionId == factionId.Value);
                }

                return units
                    .Select(u => _unitInfo.CreateDetailedUnitDto(u, snapshot, requestorFactionId))
                    .ToList();
            });

            return Ok(detailedInfoList);
        }

        /// <summary>
        /// Get detailed information about units within a radius of a location
        /// </summary>
        [HttpGet("nearby")]
        public ActionResult<List<DetailedUnitDto>> GetNearbyUnits(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusMeters = 10000,
            [FromQuery] int? requestorFactionId = null)
        {
            var snapshot = _worldState.GetSnapshot();

            var nearbyUnits = snapshot.Units
                .Where(u => DistanceMeters(latitude, longitude, u.Latitude, u.Longitude) <= radiusMeters)
                .Select(u => _unitInfo.CreateDetailedUnitDto(u, snapshot, requestorFactionId))
                .ToList();

            return Ok(nearbyUnits);
        }

        /// <summary>
        /// Get detailed information about units visible to a specific unit
        /// </summary>
        [HttpGet("{unitId}/visible")]
        public ActionResult<List<DetailedUnitDto>> GetVisibleUnits(Guid unitId)
        {
            var snapshot = _worldState.GetSnapshot();
            var unit = snapshot.Units.FirstOrDefault(u => u.Id == unitId);

            if (unit == null)
            {
                return NotFound(new
                {
                    error = $"Unit with ID {unitId} not found"
                });
            }

            var visibleUnits = snapshot.Units
                .Where(other => other.Id != unitId &&
                               other.Status != Domain.UnitStatus.Destroyed &&
                               DistanceMeters(unit.Latitude, unit.Longitude, other.Latitude, other.Longitude) <= unit.VisionRangeMeters)
                .Select(u => _unitInfo.CreateDetailedUnitDto(u, snapshot, unit.FactionId))
                .ToList();

            return Ok(visibleUnits);
        }

        private static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double metersPerDegLat = 111320.0;
            var dy = (lat2 - lat1) * metersPerDegLat;
            var avgLat = (lat1 + lat2) / 2.0 * Math.PI / 180.0;
            var metersPerDegLon = metersPerDegLat * Math.Cos(avgLat);
            var dx = (lon2 - lon1) * metersPerDegLon;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
    }
}
