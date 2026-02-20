using System.Text.Json.Serialization;

namespace WarSim.Domain.Weapons
{
    public class Weapon
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("projectileType")]
        public string ProjectileType { get; set; } = "Bullet";

        [JsonPropertyName("damage")]
        public double Damage { get; set; } = 10.0;

        [JsonPropertyName("range")]
        public double RangeMeters { get; set; } = 2000.0;

        [JsonPropertyName("projectileSpeed")]
        public double ProjectileSpeed { get; set; } = 400.0;

        [JsonPropertyName("rateOfFire")]
        public double RateOfFire { get; set; } = 1.0; // shots per second

        [JsonPropertyName("magazineSize")]
        public int MagazineSize { get; set; } = 30;

        [JsonPropertyName("totalAmmo")]
        public int TotalAmmo { get; set; } = 300;

        [JsonPropertyName("reloadTime")]
        public double ReloadTimeSeconds { get; set; } = 3.0;
    }

    public class UnitWeaponLoadout
    {
        [JsonPropertyName("unitCategory")]
        public string UnitCategory { get; set; } = string.Empty;

        [JsonPropertyName("unitSubcategory")]
        public string UnitSubcategory { get; set; } = string.Empty;

        [JsonPropertyName("weapons")]
        public List<WeaponSlot> Weapons { get; set; } = new();
    }

    public class WeaponSlot
    {
        [JsonPropertyName("weaponId")]
        public string WeaponId { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; } = 1;

        [JsonPropertyName("currentAmmo")]
        public int CurrentAmmo { get; set; }

        [JsonPropertyName("currentMagazine")]
        public int CurrentMagazine { get; set; }

        [JsonPropertyName("lastFireTime")]
        public double LastFireTime { get; set; } = 0;

        [JsonPropertyName("isReloading")]
        public bool IsReloading { get; set; } = false;

        [JsonPropertyName("reloadStartTime")]
        public double ReloadStartTime { get; set; } = 0;
    }

    public class WeaponConfig
    {
        [JsonPropertyName("weapons")]
        public List<Weapon> Weapons { get; set; } = new();

        [JsonPropertyName("loadouts")]
        public List<UnitWeaponLoadout> Loadouts { get; set; } = new();
    }
}
