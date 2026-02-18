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

        public UnitsController(WorldStateService world)
        {
            _world = world;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Unit>> GetUnits()
        {
            return Ok(_world.GetUnits());
        }

        public record MoveCommand(Guid UnitId, double Latitude, double Longitude);

        [HttpPost("move")]
        public IActionResult MoveUnit([FromBody] MoveCommand cmd)
        {
            _world.MoveUnit(cmd.UnitId, cmd.Latitude, cmd.Longitude);
            return Accepted();
        }
    }
}
