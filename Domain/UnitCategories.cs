namespace WarSim.Domain
{
    public enum UnitCategory
    {
        AIRPLANE,
        HELICOPTER,
        GROUND_UNIT,
        SHIP,
        STRUCTURE
    }

    public enum AirplaneSubcategory
    {
        Fighter,
        Interceptor,
        Multirole,
        Strike,
        Attack,
        Bomber,
        AWACS,
        Tanker,
        Transport,
        Trainer,
        Reconnaissance,
        UAV
    }

    public enum HelicopterSubcategory
    {
        AttackHelicopter,
        TransportHelicopter,
        UtilityHelicopter,
        ScoutHelicopter,
        NavalHelicopter
    }

    public enum GroundUnitSubcategory
    {
        MainBattleTank,
        InfantryFightingVehicle,
        ArmoredPersonnelCarrier,
        ReconVehicle,
        SelfPropelledArtillery,
        TowedArtillery,
        MLRS,
        AAA,
        SAMShortRange,
        SAMMediumRange,
        SAMLongRange,
        EWR,
        CommandPost,
        Logistics,
        Engineer,
        Infantry,
        CivilianVehicles,
        Trains
    }

    public enum ShipSubcategory
    {
        AircraftCarrier,
        HelicopterCarrier,
        Destroyer,
        Frigate,
        Corvette,
        PatrolBoat,
        Submarine,
        MissileBoat,
        LandingCraft,
        CivilianShip,
        TankerShip,
        CargoShip
    }

    public enum StructureSubcategory
    {
        MilitaryBuilding,
        CommandBunker,
        AmmoDepot,
        FuelDepot,
        RadarTower,
        CommunicationTower,
        AirfieldStructure,
        Hangar,
        HardenedAircraftShelter,
        RunwayObject,
        Bridge,
        CivilianBuilding,
        IndustrialBuilding,
        PowerPlant,
        StaticSAM,
        StaticAAA,
        StaticVehicle,
        Fortification
    }
}
