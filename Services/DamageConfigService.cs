using System.Text.Json;
using System.Text.Json.Serialization;

namespace WarSim.Services
{
    public class DamageConfigService
    {
        private readonly ILogger<DamageConfigService> _logger;
        private DamageConfig? _config;

        public DamageConfigService(ILogger<DamageConfigService> logger)
        {
            _logger = logger;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                var path = Path.Combine("Data", "Configs", "damage-table.json");
                var json = File.ReadAllText(path);
                _config = JsonSerializer.Deserialize<DamageConfig>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogInformation($"Loaded {_config?.DamageMultipliers.Count ?? 0} damage multipliers and {_config?.ArmorValues.Count ?? 0} armor values");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load damage config");
                _config = new DamageConfig();
            }
        }

        public double CalculateDamage(string projectileType, Domain.Unit target, double baseDamage)
        {
            var multiplier = GetDamageMultiplier(projectileType, target);
            var armor = GetArmorValue(target);

            // Damage formula: (baseDamage * multiplier) - (armor * 0.1)
            var finalDamage = (baseDamage * multiplier) - (armor * 0.1);
            return Math.Max(finalDamage, baseDamage * 0.1); // minimum 10% damage always applies
        }

        public double GetDamageMultiplier(string projectileType, Domain.Unit target)
        {
            if (_config == null)
            {
                return 1.0;
            }

            // First try exact match (category + subcategory)
            var exact = _config.DamageMultipliers.FirstOrDefault(m =>
                m.ProjectileType.Equals(projectileType, StringComparison.OrdinalIgnoreCase) &&
                m.TargetCategory.Equals(target.UnitCategory.ToString(), StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(m.TargetSubcategory) &&
                m.TargetSubcategory.Equals(target.Subcategory, StringComparison.OrdinalIgnoreCase));

            if (exact != null)
            {
                return exact.Multiplier;
            }

            // Then try category match only
            var category = _config.DamageMultipliers.FirstOrDefault(m =>
                m.ProjectileType.Equals(projectileType, StringComparison.OrdinalIgnoreCase) &&
                m.TargetCategory.Equals(target.UnitCategory.ToString(), StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(m.TargetSubcategory));

            return category?.Multiplier ?? 1.0;
        }

        public double GetArmorValue(Domain.Unit unit)
        {
            if (_config == null)
            {
                return 0;
            }

            // First try exact match
            var exact = _config.ArmorValues.FirstOrDefault(a =>
                a.UnitCategory.Equals(unit.UnitCategory.ToString(), StringComparison.OrdinalIgnoreCase) &&
                a.UnitSubcategory.Equals(unit.Subcategory, StringComparison.OrdinalIgnoreCase));

            if (exact != null)
            {
                return exact.ArmorPoints;
            }

            // Then try category default
            var category = _config.ArmorValues.FirstOrDefault(a =>
                a.UnitCategory.Equals(unit.UnitCategory.ToString(), StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(a.UnitSubcategory));

            return category?.ArmorPoints ?? 0;
        }
    }

    public class DamageConfig
    {
        [JsonPropertyName("damageMultipliers")]
        public List<DamageMultiplier> DamageMultipliers { get; set; } = new();

        [JsonPropertyName("armorValues")]
        public List<ArmorValue> ArmorValues { get; set; } = new();
    }

    public class DamageMultiplier
    {
        [JsonPropertyName("projectileType")]
        public string ProjectileType { get; set; } = string.Empty;

        [JsonPropertyName("targetCategory")]
        public string TargetCategory { get; set; } = string.Empty;

        [JsonPropertyName("targetSubcategory")]
        public string TargetSubcategory { get; set; } = string.Empty;

        [JsonPropertyName("multiplier")]
        public double Multiplier { get; set; } = 1.0;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class ArmorValue
    {
        [JsonPropertyName("unitCategory")]
        public string UnitCategory { get; set; } = string.Empty;

        [JsonPropertyName("unitSubcategory")]
        public string UnitSubcategory { get; set; } = string.Empty;

        [JsonPropertyName("armorPoints")]
        public double ArmorPoints { get; set; } = 0;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
