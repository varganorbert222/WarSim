using System.Text.Json.Serialization;

namespace WarSim.DTOs
{
    public class ScenarioDefinitionDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("mapBounds")]
        public MapBoundsDto MapBounds { get; set; } = new();

        [JsonPropertyName("centerPoint")]
        public ScenarioLocationDto CenterPoint { get; set; } = new();

        [JsonPropertyName("factions")]
        public List<FactionDefinitionDto> Factions { get; set; } = new();

        [JsonPropertyName("airbases")]
        public List<ScenarioLocationDto> Airbases { get; set; } = new();

        [JsonPropertyName("cities")]
        public List<ScenarioLocationDto> Cities { get; set; } = new();

        [JsonPropertyName("navalZones")]
        public List<ScenarioLocationDto> NavalZones { get; set; } = new();

        [JsonPropertyName("units")]
        public List<UnitDefinitionDto> Units { get; set; } = new();
    }

    public class MapBoundsDto
    {
        [JsonPropertyName("minLatitude")]
        public double MinLatitude
        {
            get; set;
        }

        [JsonPropertyName("maxLatitude")]
        public double MaxLatitude
        {
            get; set;
        }

        [JsonPropertyName("minLongitude")]
        public double MinLongitude
        {
            get; set;
        }

        [JsonPropertyName("maxLongitude")]
        public double MaxLongitude
        {
            get; set;
        }
    }

    public class ScenarioLocationDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude
        {
            get; set;
        }

        [JsonPropertyName("longitude")]
        public double Longitude
        {
            get; set;
        }

        [JsonPropertyName("faction")]
        public string Faction { get; set; } = string.Empty;
    }

    public class FactionDefinitionDto
    {
        [JsonPropertyName("id")]
        public int Id
        {
            get; set;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("allies")]
        public List<int> Allies { get; set; } = new();
    }

    public class UnitDefinitionDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("subcategory")]
        public string Subcategory { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude
        {
            get; set;
        }

        [JsonPropertyName("longitude")]
        public double Longitude
        {
            get; set;
        }

        [JsonPropertyName("heading")]
        public double Heading
        {
            get; set;
        }

        [JsonPropertyName("speed")]
        public double Speed
        {
            get; set;
        }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Idle";

        [JsonPropertyName("factionId")]
        public int FactionId
        {
            get; set;
        }

        [JsonPropertyName("health")]
        public double Health { get; set; } = 100.0;

        [JsonPropertyName("visionRangeMeters")]
        public double VisionRangeMeters { get; set; } = 2000.0;

        [JsonPropertyName("baseLocation")]
        public string BaseLocation { get; set; } = string.Empty;

        [JsonPropertyName("properties")]
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}
