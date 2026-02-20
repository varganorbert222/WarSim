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

        public ProjectilesController(WorldStateService world, ILogger<ProjectilesController> logger)
        {
            _world = world;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Projectile>> Get()
        {
            _logger.LogInformation("Get projectiles request");
            return Ok(_world.GetSnapshot().Projectiles);
        }
    }
}
