using System.Text.Json.Serialization;

namespace WarSim.DTOs
{
    public class MapDefinitionDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [JsonPropertyName("mapBounds")]
        public MapBoundsDto MapBounds { get; set; } = new();

        [JsonPropertyName("centerPoint")]
        public MapLocationDto CenterPoint { get; set; } = new();

        [JsonPropertyName("airbases")]
        public List<AirbaseDto> Airbases { get; set; } = new();

        [JsonPropertyName("cities")]
        public List<CityDto> Cities { get; set; } = new();

        [JsonPropertyName("navalZones")]
        public List<NavalZoneDto> NavalZones { get; set; } = new();

        [JsonPropertyName("strategicPoints")]
        public List<StrategicPointDto> StrategicPoints { get; set; } = new();
    }

    public class AirbaseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("runwayLength")]
        public int RunwayLength { get; set; }

        [JsonPropertyName("controlFaction")]
        public string ControlFaction { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("features")]
        public List<string> Features { get; set; } = new();
    }

    public class CityDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("population")]
        public int Population { get; set; }

        [JsonPropertyName("controlFaction")]
        public string ControlFaction { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("strategicValue")]
        public string StrategicValue { get; set; } = string.Empty;
    }

    public class NavalZoneDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("controlFaction")]
        public string ControlFaction { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("features")]
        public List<string> Features { get; set; } = new();
    }

    public class StrategicPointDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("controlFaction")]
        public string ControlFaction { get; set; } = string.Empty;
    }

    public class MapLocationDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
