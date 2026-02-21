using System.Text.Json.Serialization;

namespace WarSim.DTOs
{
    public class MissionDefinitionDto
    {
        [JsonPropertyName("missionName")]
        public string MissionName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("mapId")]
        public string MapId { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [JsonPropertyName("startTime")]
        public string StartTime { get; set; } = "08:00:00";

        [JsonPropertyName("weather")]
        public string Weather { get; set; } = "Clear";

        [JsonPropertyName("factions")]
        public List<FactionDefinitionDto> Factions { get; set; } = new();

        [JsonPropertyName("units")]
        public List<UnitDefinitionDto> Units { get; set; } = new();

        [JsonPropertyName("staticStructures")]
        public List<StaticStructureDto> StaticStructures { get; set; } = new();
    }

    public class StaticStructureDto
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

        [JsonPropertyName("factionId")]
        public int FactionId
        {
            get; set;
        }

        [JsonPropertyName("health")]
        public double Health { get; set; } = 100.0;

        [JsonPropertyName("visionRangeMeters")]
        public double VisionRangeMeters { get; set; } = 2000.0;

        [JsonPropertyName("linkedAirbase")]
        public string LinkedAirbase { get; set; } = string.Empty;

        [JsonPropertyName("linkedCity")]
        public string LinkedCity { get; set; } = string.Empty;
    }
}
