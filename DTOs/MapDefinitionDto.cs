using System.Text.Json.Serialization;

namespace WarSim.DTOs
{
    /// <summary>
    /// Map definition - static terrain data, airbases, cities, structures
    /// Separate from dynamic unit placements
    /// </summary>
    public class MapDefinitionDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("mapBounds")]
        public MapBoundsDto MapBounds { get; set; } = new();

        [JsonPropertyName("centerPoint")]
        public ScenarioLocationDto CenterPoint { get; set; } = new();

        [JsonPropertyName("airbases")]
        public List<AirbaseDefinitionDto> Airbases { get; set; } = new();

        [JsonPropertyName("cities")]
        public List<CityDefinitionDto> Cities { get; set; } = new();

        [JsonPropertyName("navalZones")]
        public List<NavalZoneDefinitionDto> NavalZones { get; set; } = new();

        [JsonPropertyName("staticStructures")]
        public List<StaticStructureDefinitionDto> StaticStructures { get; set; } = new();
    }

    public class AirbaseDefinitionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("defaultFaction")]
        public string DefaultFaction { get; set; } = string.Empty;

        [JsonPropertyName("runwayHeading")]
        public double RunwayHeading { get; set; }

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }
    }

    public class CityDefinitionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("defaultFaction")]
        public string DefaultFaction { get; set; } = string.Empty;

        [JsonPropertyName("population")]
        public int Population { get; set; }
    }

    public class NavalZoneDefinitionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("defaultFaction")]
        public string DefaultFaction { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "Port";
    }

    public class StaticStructureDefinitionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("heading")]
        public double Heading { get; set; }

        [JsonPropertyName("faction")]
        public string Faction { get; set; } = string.Empty;
    }
}
