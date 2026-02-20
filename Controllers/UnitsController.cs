using Microsoft.AspNetCore.Mvc;
using WarSim.Domain;
using WarSim.Services;

namespace WarSim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly WorldStateService _world;
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(WorldStateService world, ILogger<UnitsController> logger)
        {
            _world = world;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Unit>> GetUnits()
        {
            return Ok(_world.GetUnits());
        }

        public record MoveCommand(Guid UnitId, double Latitude, double Longitude);

        [HttpPost("move")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult MoveUnit([FromBody] MoveCommand cmd)
        {
            var moved = _world.MoveUnit(cmd.UnitId, cmd.Latitude, cmd.Longitude);
            if (!moved)
            {
                _logger.LogWarning("Move request for unknown unit {UnitId}", cmd.UnitId);
                return NotFound();
            }
            // Log to console too
            WarSim.Logging.ConsoleColorLogger.Log("API.Controllers.UnitsController", Microsoft.Extensions.Logging.LogLevel.Information, $"MoveUnit request for {cmd.UnitId}");
            return Accepted();
        }
    }
}
