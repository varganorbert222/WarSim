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
        private IEnumerable<Faction>? _cachedFactions;
        private readonly object _cacheLock = new();

        public FactionsController(FactionService factions)
        {
            _factions = factions;
        }

        /// <summary>
        /// Get all factions. Cached until factions are modified.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Faction>> Get()
        {
            lock (_cacheLock)
            {
                if (_cachedFactions == null)
                {
                    WarSim.Logging.ConsoleColorLogger.Log("API.Controllers.FactionsController", 
                        Microsoft.Extensions.Logging.LogLevel.Information, "Get factions request (cache miss)");
                    _cachedFactions = _factions.GetFactions().ToList();
                }

                return Ok(_cachedFactions);
            }
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
            lock (_cacheLock)
            {
                _cachedFactions = null;
            }

            WarSim.Logging.ConsoleColorLogger.Log("API.Controllers.FactionsController", 
                Microsoft.Extensions.Logging.LogLevel.Information, $"Updated allies for faction {id}, cache invalidated");
            return NoContent();
        }
    }
}
