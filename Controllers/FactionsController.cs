using Microsoft.AspNetCore.Mvc;
using WarSim.Domain;
using WarSim.Services;

namespace WarSim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FactionsController : ControllerBase
    {
        private readonly FactionService _factions;

        public FactionsController(FactionService factions)
        {
            _factions = factions;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Faction>> Get()
        {
            return Ok(_factions.GetFactions());
        }

        public record AlliesUpdateDto(int[] Allies);

        [HttpPut("{id}/allies")]
        public IActionResult UpdateAllies(int id, [FromBody] AlliesUpdateDto dto)
        {
            var success = _factions.UpdateAllies(id, dto.Allies?.ToList() ?? new List<int>());
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
