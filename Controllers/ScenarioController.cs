using Microsoft.AspNetCore.Mvc;
using WarSim.Data;

namespace WarSim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScenarioController : ControllerBase
    {
        /// <summary>
        /// Get Caucasus scenario metadata including airbases, cities, and naval zones.
        /// Use this data to initialize map view on client side with OpenStreetMap/Nominatim.
        /// </summary>
        [HttpGet("caucasus/metadata")]
        public ActionResult<CaucasusMetadataDto> GetCaucasusMetadata()
        {
            return Ok(new CaucasusMetadataDto
            {
                Name = "Georgia Conflict - Caucasus Theater",
                Description = "Realistic DCS World Caucasus scenario with Blue (Georgia) vs Red (Russia) forces",
                MapBounds = new MapBoundsDto
                {
                    MinLatitude = 41.0,
                    MaxLatitude = 45.5,
                    MinLongitude = 37.0,
                    MaxLongitude = 45.0
                },
                CenterPoint = new LocationDto
                {
                    Name = "Theater Center",
                    Latitude = 42.5,
                    Longitude = 42.0
                },
                Airbases = GetAirbases(),
                Cities = GetCities(),
                NavalZones = GetNavalZones()
            });
        }

        private List<LocationDto> GetAirbases()
        {
            return new List<LocationDto>
            {
                // Blue
                new() { Name = CaucasusScenarioData.Airbases.Batumi.Name, Latitude = CaucasusScenarioData.Airbases.Batumi.Latitude, Longitude = CaucasusScenarioData.Airbases.Batumi.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Airbases.Kobuleti.Name, Latitude = CaucasusScenarioData.Airbases.Kobuleti.Latitude, Longitude = CaucasusScenarioData.Airbases.Kobuleti.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Airbases.Kutaisi.Name, Latitude = CaucasusScenarioData.Airbases.Kutaisi.Latitude, Longitude = CaucasusScenarioData.Airbases.Kutaisi.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Airbases.SenakiKolkhi.Name, Latitude = CaucasusScenarioData.Airbases.SenakiKolkhi.Latitude, Longitude = CaucasusScenarioData.Airbases.SenakiKolkhi.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Airbases.Tbilisi.Name, Latitude = CaucasusScenarioData.Airbases.Tbilisi.Latitude, Longitude = CaucasusScenarioData.Airbases.Tbilisi.Longitude, Faction = "Blue" },
                // Red
                new() { Name = CaucasusScenarioData.Airbases.Gudauta.Name, Latitude = CaucasusScenarioData.Airbases.Gudauta.Latitude, Longitude = CaucasusScenarioData.Airbases.Gudauta.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Airbases.SochiBabusheri.Name, Latitude = CaucasusScenarioData.Airbases.SochiBabusheri.Latitude, Longitude = CaucasusScenarioData.Airbases.SochiBabusheri.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Airbases.Anapa.Name, Latitude = CaucasusScenarioData.Airbases.Anapa.Latitude, Longitude = CaucasusScenarioData.Airbases.Anapa.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Airbases.Gelendzhik.Name, Latitude = CaucasusScenarioData.Airbases.Gelendzhik.Latitude, Longitude = CaucasusScenarioData.Airbases.Gelendzhik.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Airbases.Krasnodar.Name, Latitude = CaucasusScenarioData.Airbases.Krasnodar.Latitude, Longitude = CaucasusScenarioData.Airbases.Krasnodar.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Airbases.Mineralnye.Name, Latitude = CaucasusScenarioData.Airbases.Mineralnye.Latitude, Longitude = CaucasusScenarioData.Airbases.Mineralnye.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Airbases.Mozdok.Name, Latitude = CaucasusScenarioData.Airbases.Mozdok.Latitude, Longitude = CaucasusScenarioData.Airbases.Mozdok.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Airbases.Beslan.Name, Latitude = CaucasusScenarioData.Airbases.Beslan.Latitude, Longitude = CaucasusScenarioData.Airbases.Beslan.Longitude, Faction = "Red" }
            };
        }

        private List<LocationDto> GetCities()
        {
            return new List<LocationDto>
            {
                new() { Name = CaucasusScenarioData.Cities.BatumiCity.Name, Latitude = CaucasusScenarioData.Cities.BatumiCity.Latitude, Longitude = CaucasusScenarioData.Cities.BatumiCity.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Cities.Poti.Name, Latitude = CaucasusScenarioData.Cities.Poti.Latitude, Longitude = CaucasusScenarioData.Cities.Poti.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Cities.Zugdidi.Name, Latitude = CaucasusScenarioData.Cities.Zugdidi.Latitude, Longitude = CaucasusScenarioData.Cities.Zugdidi.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Cities.TbilisiCity.Name, Latitude = CaucasusScenarioData.Cities.TbilisiCity.Latitude, Longitude = CaucasusScenarioData.Cities.TbilisiCity.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Cities.Gori.Name, Latitude = CaucasusScenarioData.Cities.Gori.Latitude, Longitude = CaucasusScenarioData.Cities.Gori.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.Cities.Sukhumi.Name, Latitude = CaucasusScenarioData.Cities.Sukhumi.Latitude, Longitude = CaucasusScenarioData.Cities.Sukhumi.Longitude, Faction = "Contested" },
                new() { Name = CaucasusScenarioData.Cities.Sochi.Name, Latitude = CaucasusScenarioData.Cities.Sochi.Latitude, Longitude = CaucasusScenarioData.Cities.Sochi.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Cities.Tuapse.Name, Latitude = CaucasusScenarioData.Cities.Tuapse.Latitude, Longitude = CaucasusScenarioData.Cities.Tuapse.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Cities.Novorossiysk.Name, Latitude = CaucasusScenarioData.Cities.Novorossiysk.Latitude, Longitude = CaucasusScenarioData.Cities.Novorossiysk.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.Cities.Vladikavkaz.Name, Latitude = CaucasusScenarioData.Cities.Vladikavkaz.Latitude, Longitude = CaucasusScenarioData.Cities.Vladikavkaz.Longitude, Faction = "Red" }
            };
        }

        private List<LocationDto> GetNavalZones()
        {
            return new List<LocationDto>
            {
                new() { Name = CaucasusScenarioData.NavalZones.BatumiPort.Name, Latitude = CaucasusScenarioData.NavalZones.BatumiPort.Latitude, Longitude = CaucasusScenarioData.NavalZones.BatumiPort.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.NavalZones.PotiPort.Name, Latitude = CaucasusScenarioData.NavalZones.PotiPort.Latitude, Longitude = CaucasusScenarioData.NavalZones.PotiPort.Longitude, Faction = "Blue" },
                new() { Name = CaucasusScenarioData.NavalZones.SukhumiPort.Name, Latitude = CaucasusScenarioData.NavalZones.SukhumiPort.Latitude, Longitude = CaucasusScenarioData.NavalZones.SukhumiPort.Longitude, Faction = "Contested" },
                new() { Name = CaucasusScenarioData.NavalZones.SochiPort.Name, Latitude = CaucasusScenarioData.NavalZones.SochiPort.Latitude, Longitude = CaucasusScenarioData.NavalZones.SochiPort.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.NavalZones.TuapsePort.Name, Latitude = CaucasusScenarioData.NavalZones.TuapsePort.Latitude, Longitude = CaucasusScenarioData.NavalZones.TuapsePort.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.NavalZones.NovorossiyskPort.Name, Latitude = CaucasusScenarioData.NavalZones.NovorossiyskPort.Latitude, Longitude = CaucasusScenarioData.NavalZones.NovorossiyskPort.Longitude, Faction = "Red" },
                new() { Name = CaucasusScenarioData.NavalZones.AnapaWaters.Name, Latitude = CaucasusScenarioData.NavalZones.AnapaWaters.Latitude, Longitude = CaucasusScenarioData.NavalZones.AnapaWaters.Longitude, Faction = "Red" }
            };
        }
    }

    public class CaucasusMetadataDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MapBoundsDto MapBounds { get; set; } = new();
        public LocationDto CenterPoint { get; set; } = new();
        public List<LocationDto> Airbases { get; set; } = new();
        public List<LocationDto> Cities { get; set; } = new();
        public List<LocationDto> NavalZones { get; set; } = new();
    }

    public class MapBoundsDto
    {
        public double MinLatitude
        {
            get; set;
        }
        public double MaxLatitude
        {
            get; set;
        }
        public double MinLongitude
        {
            get; set;
        }
        public double MaxLongitude
        {
            get; set;
        }
    }

    public class LocationDto
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public string Faction { get; set; } = string.Empty;
    }
}
