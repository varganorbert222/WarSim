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
        private readonly ResponseCacheService _cache;

        public FactionsController(FactionService factions, ResponseCacheService cache)
        {
            _factions = factions;
            _cache = cache;
        }

        /// <summary>
        /// Get all factions. Permanently cached until invalidated.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Faction>> Get()
        {
            var factions = _cache.GetOrCreatePermanent("factions_all", () =>
            {
                WarSim.Logging.ConsoleColorLogger.Log("API.Controllers.FactionsController",
                    Microsoft.Extensions.Logging.LogLevel.Information, "Get factions request (cache miss)");
                return _factions.GetFactions().ToList();
            });

            return Ok(factions);
        }

        public record AlliesUpdateDto(int[] Allies);

        [HttpPut("{id}/allies")]
        public IActionResult UpdateAllies(int id, [FromBody] AlliesUpdateDto dto)
        {
            var success = _factions.UpdateAllies(id, dto.Allies?.ToList() ?? new List<int>());
            if (!success)
            {
                return NotFound();
            }

            // Invalidate cache when factions are modified
            _cache.Invalidate("factions_all");

            WarSim.Logging.ConsoleColorLogger.Log("API.Controllers.FactionsController",
                Microsoft.Extensions.Logging.LogLevel.Information, $"Updated allies for faction {id}, cache invalidated");
            return NoContent();
        }
    }
}
