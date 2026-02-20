using WarSim.Domain;
using WarSim.Domain.Units;
using WarSim.Domain.Projectiles;
using WarSim.Data;

namespace WarSim.Services
{
    /// <summary>
    /// Service for creating realistic scenario setups.
    /// Caucasus scenario: Blue (Georgia) vs Red (Russia)
    /// </summary>
    public class ScenarioService
    {
        public WorldState CreateCaucasusScenario()
        {
            var units = new List<Unit>();
            var projectiles = new List<Projectile>();
            var factions = new List<Faction>
            {
                new() { Id = 1, Name = "Georgia (Blue)", Color = "#0066CC", Allies = new List<int> { 1 } },
                new() { Id = 2, Name = "Russia (Red)", Color = "#CC0000", Allies = new List<int> { 2 } }
            };

            // Blue forces (Georgia)
            units.AddRange(CreateBlueAirForces());
            units.AddRange(CreateBlueGroundForces());
            units.AddRange(CreateBlueNavalForces());
            units.AddRange(CreateBlueStructures());

            // Red forces (Russia)
            units.AddRange(CreateRedAirForces());
            units.AddRange(CreateRedGroundForces());
            units.AddRange(CreateRedNavalForces());
            units.AddRange(CreateRedStructures());

            return new WorldState(units, projectiles, factions, 0);
        }

        private List<Unit> CreateBlueAirForces()
        {
            var units = new List<Unit>();

            // Batumi - F-16 CAP
            units.Add(new Aircraft(AirplaneSubcategory.Multirole)
            {
                Name = "Viper 1-1",
                Latitude = CaucasusScenarioData.Airbases.Batumi.Latitude,
                Longitude = CaucasusScenarioData.Airbases.Batumi.Longitude,
                Heading = 90,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 100,
                VisionRangeMeters = 8000,
                Capacity = 1
            });

            units.Add(new Aircraft(AirplaneSubcategory.Multirole)
            {
                Name = "Viper 1-2",
                Latitude = CaucasusScenarioData.Airbases.Batumi.Latitude + 0.002,
                Longitude = CaucasusScenarioData.Airbases.Batumi.Longitude + 0.001,
                Heading = 90,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 100,
                VisionRangeMeters = 8000,
                Capacity = 1
            });

            // Kobuleti - Su-25 Ground Attack
            units.Add(new Aircraft(AirplaneSubcategory.Attack)
            {
                Name = "Grach 2-1",
                Latitude = CaucasusScenarioData.Airbases.Kobuleti.Latitude,
                Longitude = CaucasusScenarioData.Airbases.Kobuleti.Longitude,
                Heading = 180,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 100,
                VisionRangeMeters = 5000,
                Capacity = 1
            });

            // Kutaisi - Recon UAV
            units.Add(new Aircraft(AirplaneSubcategory.UAV)
            {
                Name = "Eye-1",
                Latitude = CaucasusScenarioData.Airbases.Kutaisi.Latitude,
                Longitude = CaucasusScenarioData.Airbases.Kutaisi.Longitude,
                Heading = 45,
                Airspeed = 80,
                Status = UnitStatus.Moving,
                FactionId = 1,
                Health = 50,
                VisionRangeMeters = 12000,
                Capacity = 0
            });

            // Senaki - Transport helicopter
            units.Add(new Helicopter(HelicopterSubcategory.TransportHelicopter)
            {
                Name = "Dustoff 3-1",
                Latitude = CaucasusScenarioData.Airbases.SenakiKolkhi.Latitude,
                Longitude = CaucasusScenarioData.Airbases.SenakiKolkhi.Longitude,
                Heading = 270,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 80,
                VisionRangeMeters = 3000
            });

            return units;
        }

        private List<Unit> CreateBlueGroundForces()
        {
            var units = new List<Unit>();

            // Batumi City - Infantry
            units.Add(new Infantry()
            {
                Name = "1st Rifle Platoon",
                Latitude = CaucasusScenarioData.Cities.BatumiCity.Latitude,
                Longitude = CaucasusScenarioData.Cities.BatumiCity.Longitude,
                Heading = 0,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 100,
                VisionRangeMeters = 800,
                Strength = 30
            });

            // Gori - Main Battle Tanks
            units.Add(new Vehicle(GroundUnitSubcategory.MainBattleTank)
            {
                Name = "Tank Platoon Alpha",
                Latitude = CaucasusScenarioData.Cities.Gori.Latitude,
                Longitude = CaucasusScenarioData.Cities.Gori.Longitude,
                Heading = 45,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 150,
                VisionRangeMeters = 3000,
                Crew = 4
            });

            // Tbilisi - SAM Long Range
            units.Add(new Vehicle(GroundUnitSubcategory.SAMLongRange)
            {
                Name = "Patriot Battery 1",
                Latitude = CaucasusScenarioData.Cities.TbilisiCity.Latitude,
                Longitude = CaucasusScenarioData.Cities.TbilisiCity.Longitude,
                Heading = 180,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 120,
                VisionRangeMeters = 15000,
                Crew = 6
            });

            // Poti - AAA
            units.Add(new Vehicle(GroundUnitSubcategory.AAA)
            {
                Name = "Shilka Defense 2",
                Latitude = CaucasusScenarioData.Cities.Poti.Latitude,
                Longitude = CaucasusScenarioData.Cities.Poti.Longitude,
                Heading = 90,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 100,
                VisionRangeMeters = 5000,
                Crew = 4
            });

            return units;
        }

        private List<Unit> CreateBlueNavalForces()
        {
            var units = new List<Unit>();

            // Batumi Port - Frigate
            units.Add(new Ship(ShipSubcategory.Frigate)
            {
                Name = "GNS Dioskuria",
                Latitude = CaucasusScenarioData.NavalZones.BatumiPort.Latitude,
                Longitude = CaucasusScenarioData.NavalZones.BatumiPort.Longitude,
                Heading = 270,
                SpeedKnots = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 200,
                VisionRangeMeters = 10000,
                Crew = 150
            });

            // Poti Port - Patrol Boat
            units.Add(new Ship(ShipSubcategory.PatrolBoat)
            {
                Name = "PB-201",
                Latitude = CaucasusScenarioData.NavalZones.PotiPort.Latitude,
                Longitude = CaucasusScenarioData.NavalZones.PotiPort.Longitude,
                Heading = 180,
                SpeedKnots = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 80,
                VisionRangeMeters = 6000,
                Crew = 25
            });

            return units;
        }

        private List<Unit> CreateBlueStructures()
        {
            var units = new List<Unit>();

            // Batumi - Radar Tower
            units.Add(new Structure(StructureSubcategory.RadarTower)
            {
                Name = "Batumi ATC Radar",
                Latitude = CaucasusScenarioData.Airbases.Batumi.Latitude + 0.01,
                Longitude = CaucasusScenarioData.Airbases.Batumi.Longitude + 0.01,
                Heading = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 150,
                VisionRangeMeters = 20000
            });

            // Kutaisi - Hangar
            units.Add(new Structure(StructureSubcategory.Hangar)
            {
                Name = "Kutaisi Hangar 1",
                Latitude = CaucasusScenarioData.Airbases.Kutaisi.Latitude - 0.005,
                Longitude = CaucasusScenarioData.Airbases.Kutaisi.Longitude + 0.005,
                Heading = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 200,
                VisionRangeMeters = 500
            });

            // Tbilisi - Command Bunker
            units.Add(new Structure(StructureSubcategory.CommandBunker)
            {
                Name = "Tbilisi Command Center",
                Latitude = CaucasusScenarioData.Cities.TbilisiCity.Latitude,
                Longitude = CaucasusScenarioData.Cities.TbilisiCity.Longitude + 0.02,
                Heading = 0,
                Status = UnitStatus.Idle,
                FactionId = 1,
                Health = 300,
                VisionRangeMeters = 5000
            });

            return units;
        }

        private List<Unit> CreateRedAirForces()
        {
            var units = new List<Unit>();

            // Anapa - Su-27 Interceptors
            units.Add(new Aircraft(AirplaneSubcategory.Interceptor)
            {
                Name = "Flanker 1-1",
                Latitude = CaucasusScenarioData.Airbases.Anapa.Latitude,
                Longitude = CaucasusScenarioData.Airbases.Anapa.Longitude,
                Heading = 180,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 100,
                VisionRangeMeters = 10000,
                Capacity = 1
            });

            units.Add(new Aircraft(AirplaneSubcategory.Interceptor)
            {
                Name = "Flanker 1-2",
                Latitude = CaucasusScenarioData.Airbases.Anapa.Latitude + 0.003,
                Longitude = CaucasusScenarioData.Airbases.Anapa.Longitude,
                Heading = 180,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 100,
                VisionRangeMeters = 10000,
                Capacity = 1
            });

            // Mozdok - Su-25 Ground Attack
            units.Add(new Aircraft(AirplaneSubcategory.Attack)
            {
                Name = "Frogfoot 2-1",
                Latitude = CaucasusScenarioData.Airbases.Mozdok.Latitude,
                Longitude = CaucasusScenarioData.Airbases.Mozdok.Longitude,
                Heading = 270,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 100,
                VisionRangeMeters = 5000,
                Capacity = 1
            });

            // Mineralnye - AWACS
            units.Add(new Aircraft(AirplaneSubcategory.AWACS)
            {
                Name = "Mainstay 3-1",
                Latitude = CaucasusScenarioData.Airbases.Mineralnye.Latitude,
                Longitude = CaucasusScenarioData.Airbases.Mineralnye.Longitude + 0.5,
                Heading = 90,
                Airspeed = 450,
                Status = UnitStatus.Moving,
                FactionId = 2,
                Health = 120,
                VisionRangeMeters = 50000,
                Capacity = 10
            });

            // Gudauta - Attack Helicopter
            units.Add(new Helicopter(HelicopterSubcategory.AttackHelicopter)
            {
                Name = "Havoc 4-1",
                Latitude = CaucasusScenarioData.Airbases.Gudauta.Latitude,
                Longitude = CaucasusScenarioData.Airbases.Gudauta.Longitude,
                Heading = 90,
                Airspeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 90,
                VisionRangeMeters = 4000
            });

            return units;
        }

        private List<Unit> CreateRedGroundForces()
        {
            var units = new List<Unit>();

            // Sukhumi - Infantry
            units.Add(new Infantry()
            {
                Name = "VDV Airborne Company",
                Latitude = CaucasusScenarioData.Cities.Sukhumi.Latitude,
                Longitude = CaucasusScenarioData.Cities.Sukhumi.Longitude,
                Heading = 180,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 100,
                VisionRangeMeters = 1000,
                Strength = 80
            });

            // Vladikavkaz - Main Battle Tanks
            units.Add(new Vehicle(GroundUnitSubcategory.MainBattleTank)
            {
                Name = "T-90 Platoon Bravo",
                Latitude = CaucasusScenarioData.Cities.Vladikavkaz.Latitude,
                Longitude = CaucasusScenarioData.Cities.Vladikavkaz.Longitude,
                Heading = 270,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 180,
                VisionRangeMeters = 3500,
                Crew = 3
            });

            // Mozdok - SAM Long Range
            units.Add(new Vehicle(GroundUnitSubcategory.SAMLongRange)
            {
                Name = "S-300 Battery 5",
                Latitude = CaucasusScenarioData.Airbases.Mozdok.Latitude + 0.02,
                Longitude = CaucasusScenarioData.Airbases.Mozdok.Longitude,
                Heading = 180,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 150,
                VisionRangeMeters = 18000,
                Crew = 8
            });

            // Gelendzhik - MLRS
            units.Add(new Vehicle(GroundUnitSubcategory.MLRS)
            {
                Name = "Smerch Battery 1",
                Latitude = CaucasusScenarioData.Airbases.Gelendzhik.Latitude - 0.03,
                Longitude = CaucasusScenarioData.Airbases.Gelendzhik.Longitude,
                Heading = 225,
                GroundSpeed = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 100,
                VisionRangeMeters = 2000,
                Crew = 6
            });

            return units;
        }

        private List<Unit> CreateRedNavalForces()
        {
            var units = new List<Unit>();

            // Novorossiysk - Destroyer
            units.Add(new Ship(ShipSubcategory.Destroyer)
            {
                Name = "RFS Smetlivy",
                Latitude = CaucasusScenarioData.NavalZones.NovorossiyskPort.Latitude,
                Longitude = CaucasusScenarioData.NavalZones.NovorossiyskPort.Longitude,
                Heading = 90,
                SpeedKnots = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 250,
                VisionRangeMeters = 12000,
                Crew = 280
            });

            // Tuapse - Corvette
            units.Add(new Ship(ShipSubcategory.Corvette)
            {
                Name = "RFS Merkury",
                Latitude = CaucasusScenarioData.NavalZones.TuapsePort.Latitude,
                Longitude = CaucasusScenarioData.NavalZones.TuapsePort.Longitude,
                Heading = 180,
                SpeedKnots = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 150,
                VisionRangeMeters = 8000,
                Crew = 60
            });

            // Sochi - Submarine
            units.Add(new Ship(ShipSubcategory.Submarine)
            {
                Name = "RFS Kilo-636",
                Latitude = CaucasusScenarioData.NavalZones.SochiPort.Latitude - 0.05,
                Longitude = CaucasusScenarioData.NavalZones.SochiPort.Longitude,
                Heading = 270,
                SpeedKnots = 5,
                Status = UnitStatus.Moving,
                FactionId = 2,
                Health = 120,
                VisionRangeMeters = 15000,
                Crew = 52
            });

            return units;
        }

        private List<Unit> CreateRedStructures()
        {
            var units = new List<Unit>();

            // Anapa - EWR Radar
            units.Add(new Structure(StructureSubcategory.RadarTower)
            {
                Name = "Anapa Early Warning Radar",
                Latitude = CaucasusScenarioData.Airbases.Anapa.Latitude + 0.015,
                Longitude = CaucasusScenarioData.Airbases.Anapa.Longitude + 0.01,
                Heading = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 180,
                VisionRangeMeters = 30000
            });

            // Mozdok - Hardened Aircraft Shelter
            units.Add(new Structure(StructureSubcategory.HardenedAircraftShelter)
            {
                Name = "Mozdok HAS-3",
                Latitude = CaucasusScenarioData.Airbases.Mozdok.Latitude - 0.01,
                Longitude = CaucasusScenarioData.Airbases.Mozdok.Longitude + 0.008,
                Heading = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 350,
                VisionRangeMeters = 200
            });

            // Sochi - Communication Tower
            units.Add(new Structure(StructureSubcategory.CommunicationTower)
            {
                Name = "Sochi Comms Relay",
                Latitude = CaucasusScenarioData.Cities.Sochi.Latitude + 0.02,
                Longitude = CaucasusScenarioData.Cities.Sochi.Longitude,
                Heading = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 100,
                VisionRangeMeters = 8000
            });

            // Beslan - Static SAM
            units.Add(new Structure(StructureSubcategory.StaticSAM)
            {
                Name = "Beslan SA-6 Site",
                Latitude = CaucasusScenarioData.Airbases.Beslan.Latitude + 0.02,
                Longitude = CaucasusScenarioData.Airbases.Beslan.Longitude - 0.01,
                Heading = 0,
                Status = UnitStatus.Idle,
                FactionId = 2,
                Health = 200,
                VisionRangeMeters = 12000
            });

            return units;
        }
    }
}
