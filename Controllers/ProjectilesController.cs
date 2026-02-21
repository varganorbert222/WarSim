using Microsoft.AspNetCore.Mvc;
using WarSim.Domain.Projectiles;
using WarSim.Services;

namespace WarSim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectilesController : ControllerBase
    {
        private readonly WorldStateService _world;
        private readonly ILogger<ProjectilesController> _logger;
        private IEnumerable<Projectile>? _cachedProjectiles;
        private long _cachedTick = -1;
        private readonly object _cacheLock = new();

        public ProjectilesController(WorldStateService world, ILogger<ProjectilesController> logger)
        {
            _world = world;
            _logger = logger;
        }

        /// <summary>
        /// Get all projectiles. Cached based on world state tick.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Projectile>> Get()
        {
            var snapshot = _world.GetSnapshot();

            lock (_cacheLock)
            {
                if (_cachedProjectiles != null && _cachedTick == snapshot.Tick)
                {
                    _logger.LogDebug("Projectiles cache hit for tick {Tick}", snapshot.Tick);
                    return Ok(_cachedProjectiles);
                }

                _logger.LogInformation("Get projectiles request (cache miss, tick: {Tick})", snapshot.Tick);
                _cachedProjectiles = snapshot.Projectiles.ToList(); // ToList to avoid multiple enumerations
                _cachedTick = snapshot.Tick;

                return Ok(_cachedProjectiles);
            }
        }
    }
}
