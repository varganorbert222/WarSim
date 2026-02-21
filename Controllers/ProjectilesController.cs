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
        private readonly ResponseCacheService _cache;

        public ProjectilesController(WorldStateService world, ILogger<ProjectilesController> logger, ResponseCacheService cache)
        {
            _world = world;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Get all projectiles. Cached based on world state tick.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Projectile>> Get()
        {
            var projectiles = _cache.GetOrCreate("projectiles_all", () =>
            {
                _logger.LogInformation("Get projectiles request (cache miss, tick: {Tick})", _world.GetSnapshot().Tick);
                return _world.GetSnapshot().Projectiles.ToList();
            });

            return Ok(projectiles);
        }
    }
}
