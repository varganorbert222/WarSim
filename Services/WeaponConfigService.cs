using System.Text.Json;
using WarSim.Domain.Weapons;
using Microsoft.Extensions.Logging;

namespace WarSim.Services
{
    public class WeaponConfigService
    {
        private readonly ILogger<WeaponConfigService> _logger;
        private WeaponConfig? _config;

        public WeaponConfigService(ILogger<WeaponConfigService> logger)
        {
            _logger = logger;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                var path = Path.Combine("Data", "Configs", "weapons.json");
                var json = File.ReadAllText(path);
                _config = JsonSerializer.Deserialize<WeaponConfig>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogInformation($"Loaded {_config?.Weapons.Count ?? 0} weapons and {_config?.Loadouts.Count ?? 0} loadouts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load weapon config");
                _config = new WeaponConfig();
            }
        }

        public Weapon? GetWeapon(string weaponId)
        {
            return _config?.Weapons.FirstOrDefault(w => w.Id == weaponId);
        }

        public UnitWeaponLoadout? GetLoadoutForUnit(string category, string subcategory)
        {
            return _config?.Loadouts.FirstOrDefault(l =>
                l.UnitCategory.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                l.UnitSubcategory.Equals(subcategory, StringComparison.OrdinalIgnoreCase));
        }

        public List<Weapon> GetAllWeapons()
        {
            return _config?.Weapons ?? new List<Weapon>();
        }
    }
}
